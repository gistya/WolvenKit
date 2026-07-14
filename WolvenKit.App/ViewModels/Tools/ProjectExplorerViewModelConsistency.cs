using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WolvenKit.App.Helpers;
using WolvenKit.App.Models;

namespace WolvenKit.App.ViewModels.Tools;

public partial class ProjectExplorerViewModel
{
    // The single source of truth for the project explorer's two grids. Their ItemsSource
    // collections (FileTree, FileList) live on this object, and it runs the GridFlow machine that
    // says what mode the UI is in. It's constructed here and handed to the WatcherService, so there
    // is exactly one owner of those collections rather than the watcher minting its own.
    private readonly GridGuard _gridGuard = new();

    /// <summary>The grid source-of-truth: owns FileTree/FileList and runs the GridFlow machine.</summary>
    public GridGuard Guard => _gridGuard;

    /// <summary>
    /// True while a high-level operation owns the grids (the machine is not in Ready). External
    /// writers should check this before touching the grids or the models behind their ItemsSource.
    /// </summary>
    public bool GridsLocked => _gridGuard.GridsLocked;
}

public static class GridFlow
{
    public enum State { Ready, MakingChangesToFiles, AwaitingRedrawsOfGrids }

    public enum Event
    {
        ChangeRequestReceived,
        ChangeRequestDenialSent,
        ChangeRequestApprovalSent,
        ChangeRequestCarriedOut,
        RequestedChangesConfirmedMade,
        GridUpdateConfirmedCompleted,
    }

    /// <summary>
    /// THE machine, fully typed and declared in pure C# right here. This is the single source of truth
    /// for all transitions. Self-targets are "legal in this state but produce no mode change".
    /// The GridGuard below is a tiny interpreter over this table (no external libraries or native bridges).
    /// </summary>
    internal static readonly Dictionary<State, Dictionary<Event, State>> Table = new()
    {
        [State.Ready] = new()
        {
            [Event.ChangeRequestReceived]    = State.Ready,
            [Event.ChangeRequestDenialSent]  = State.Ready,
            [Event.ChangeRequestApprovalSent] = State.MakingChangesToFiles,
        },
        [State.MakingChangesToFiles] = new()
        {
            [Event.ChangeRequestCarriedOut]       = State.MakingChangesToFiles,
            [Event.RequestedChangesConfirmedMade] = State.AwaitingRedrawsOfGrids,
        },
        [State.AwaitingRedrawsOfGrids] = new()
        {
            [Event.GridUpdateConfirmedCompleted] = State.Ready,
        },
    };

    public const State Initial = State.Ready;

    /// <summary>
    /// Returns true if (current, evt) has a defined transition in the table; writes the target state.
    /// The caller (GridGuard) applies it and decides whether an actual mode change occurred.
    /// </summary>
    public static bool TryTransition(State current, Event evt, out State next)
    {
        if (Table.TryGetValue(current, out var map) && map.TryGetValue(evt, out next))
            return true;
        next = default;
        return false;
    }

    // Wire string and TryParseState are retained for any string-based callers or debugging.
    // They are no longer used by the runtime (we use the enums + Table directly).
    public static string Wire<T>(T value) where T : Enum
    {
        var name = value.ToString();
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }

    public static bool TryParseState(string wire, out State state) =>
        Enum.TryParse(wire, ignoreCase: true, out state);
}

/// <summary>
/// GridGuard is the single source of truth for ProjectExplorerView's two grids: the hierarchical
/// TreeGrid (<see cref="FileTree"/>) and the flat DataGrid (<see cref="FileList"/>).
///
/// It has two jobs:
///   1. It OWNS the node objects the grids bind to. These are CLONES — separate FileSystemModel
///      instances from the "domain" models the watcher builds and the rest of the app mutates. The
///      grids never bind to the wild domain models, so when other code renames a model, clears its
///      Children, or removes one, the grids don't see it and don't corrupt. The watcher reports each
///      approved domain change here via the Project* intents, and the shim re-applies it to the
///      clones on the UI thread. One owner, one truth.
///   2. It runs the GridFlow state machine (a tiny pure-C# interpreter over <see cref="GridFlow.Table"/>)
///      tracking what the UI is doing right now: Ready, MakingChangesToFiles, or AwaitingRedrawsOfGrids.
///      Writers gate on that mode instead of poking the grids whenever they feel like it.
///
/// The mode maps onto how the project explorer already behaves:
///   Ready                  - the watcher is live, the grids are safe to read and redraw.
///   MakingChangesToFiles   - a high-level operation suspended the watcher and is changing files.
///   AwaitingRedrawsOfGrids - the change is done; the grids are reconciling (the DeferRefresh window).
///
/// Clones are keyed by FullName (the absolute path), which matches the watcher's own _fileLookup,
/// so resolving a grid clone back to its domain model (e.g. for a delete) is a path lookup.
/// </summary>
public sealed class GridGuard : IDisposable
{
    /// <summary>Single source of truth for the flat DataGrid (TreeGridFlat.ItemsSource). Holds clones.</summary>
    public DispatchedObservableCollection<FileSystemModel> FileList { get; } = new();

    /// <summary>Single source of truth for the hierarchical TreeGrid (TreeGrid.ItemsSource). Holds clones.</summary>
    public DispatchedObservableCollection<FileSystemModel> FileTree { get; } = new();

    // --- Pure C# GridFlow state machine (no native bridge, no external DLLs) --------------------
    private GridFlow.State _current = GridFlow.Initial;
    private readonly object _gate = new();

    /// <summary>
    /// Invisible root clone used only for hierarchy/FullName computation in the projection.
    /// Mirrors the domain's &lt;ProjectDir&gt; so that top-level folders ("archive", "raw", ...)
    /// and their descendants compute the exact same FullName as their domain counterparts.
    /// This makes _cloneByKey lookups by domain.FullName reliable and prevents duplicate clones.
    /// </summary>
    private FileSystemModel? _invisibleCloneRoot;

    /// <summary>Raised whenever the machine's mode actually changes to a different state.</summary>
    public event Action<GridFlow.State>? ModeChanged;

    public GridGuard()
    {
        // Pure declaration: initial state is GridFlow.Initial (Ready). No bridge, no JSON, no handles.
    }

    public GridFlow.State Mode
    {
        get { lock (_gate) { return _current; } }
    }

    public bool Is(GridFlow.State s)
    {
        lock (_gate) { return _current == s; }
    }

    /// <summary>True whenever we are NOT in Ready, i.e. external writers must not touch the grids.</summary>
    public bool GridsLocked => !Is(GridFlow.State.Ready);

    /// <summary>
    /// Send a raw event. True if the event is legal in the current state per GridFlow.Table (applied).
    /// If the transition targets a different state, ModeChanged is raised (outside the lock).
    /// </summary>
    public bool Send(GridFlow.Event e)
    {
        GridFlow.State? changedTo = null;
        bool accepted;
        lock (_gate)
        {
            accepted = GridFlow.TryTransition(_current, e, out var next);
            if (accepted && next != _current)
            {
                _current = next;
                changedTo = next;
            }
        }

        if (changedTo.HasValue)
            ModeChanged?.Invoke(changedTo.Value);

        return accepted;
    }

    // --- Named transitions (these read better at call sites than the raw events) -----------------

    /// <summary>Ready -> Ready: a writer is asking to change files. Logged; no mode change.</summary>
    public bool NotifyChangeRequested() => Send(GridFlow.Event.ChangeRequestReceived);

    /// <summary>Ready -> Ready: the request was denied. No mode change.</summary>
    public bool DenyChange() => Send(GridFlow.Event.ChangeRequestDenialSent);

    /// <summary>Ready -> MakingChangesToFiles. False if we weren't in Ready (someone else holds it).</summary>
    public bool BeginChanges() => Send(GridFlow.Event.ChangeRequestApprovalSent);

    /// <summary>MakingChangesToFiles -> self: a progress tick while a change is carried out.</summary>
    public bool NotifyChangeCarriedOut() => Send(GridFlow.Event.ChangeRequestCarriedOut);

    /// <summary>MakingChangesToFiles -> AwaitingRedrawsOfGrids.</summary>
    public bool ConfirmChangesMade() => Send(GridFlow.Event.RequestedChangesConfirmedMade);

    /// <summary>AwaitingRedrawsOfGrids -> Ready.</summary>
    public bool ConfirmRedrawComplete() => Send(GridFlow.Event.GridUpdateConfirmedCompleted);

    /// <summary>
    /// Force the machine back to Ready from wherever it is. Each Send is a no-op when illegal in the
    /// current mode, so this safely lands us in Ready whether we were Making, Awaiting, or already
    /// Ready. Used by the resume/refresh choke points so an operation can never strand the guard.
    /// </summary>
    public void ForceReady()
    {
        ConfirmChangesMade();     // Making -> Awaiting (no-op otherwise)
        ConfirmRedrawComplete();  // Awaiting -> Ready  (no-op otherwise)
    }

    /// <summary>
    /// Runs the full Ready -> MakingChanges -> AwaitingRedraws -> Ready cycle around a mutation.
    /// <paramref name="mutate"/> changes files/models; <paramref name="redraw"/> (optional) does the
    /// grid reconcile. If we cannot enter MakingChanges (the guard is busy), returns false and runs
    /// nothing rather than risk corruption. The machine is always walked back to Ready in the
    /// finally, so a throw can't strand it.
    /// </summary>
    public bool RunGuardedChange(Action mutate, Action? redraw = null)
    {
        if (!BeginChanges()) return false; // someone else holds the guard; bail
        try
        {
            mutate();
            ConfirmChangesMade();  // -> AwaitingRedrawsOfGrids
            redraw?.Invoke();
            return true;
        }
        finally
        {
            ForceReady();
        }
    }

    /// <summary>Async sibling of <see cref="RunGuardedChange"/> for command handlers that await.</summary>
    public async Task<bool> RunGuardedChangeAsync(Func<Task> mutate, Func<Task>? redraw = null)
    {
        if (!BeginChanges()) return false;
        try
        {
            await mutate().ConfigureAwait(false);
            ConfirmChangesMade();
            if (redraw is not null) await redraw().ConfigureAwait(false);
            return true;
        }
        finally
        {
            ForceReady();
        }
    }

    // ===========================================================================================
    //  Projection: the grids bind to the clone nodes below; the watcher reports domain changes via
    //  the Project* intents and the shim re-applies them to the clones. Every mutation of a bound
    //  collection happens inside DispatcherHelper.RunOnMainThread so the grids only ever see changes
    //  on the UI thread, and under _projGate so the clone map and collections stay consistent.
    // ===========================================================================================

    private readonly object _projGate = new();

    // FullName (absolute path) -> the single clone for that node. Shared between FileTree and FileList.
    private readonly Dictionary<string, FileSystemModel> _cloneByKey = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Resolve a grid clone (e.g. the current selection) back to the key callers can hand
    /// the watcher to find the domain model. The clone and its domain model share a FullName, so
    /// callers normally just use clone.FullName directly; this is here for symmetry/readability.</summary>
    public string KeyOf(FileSystemModel gridNode) => gridNode.FullName;

    /// <summary>True if a clone exists for this path (i.e. the node is currently shown in the grids).</summary>
    public bool HasNode(string fullName)
    {
        lock (_projGate) { return _cloneByKey.ContainsKey(fullName); }
    }

    /// <summary>
    /// Rebuild the entire projection from a freshly built domain tree (used on project load/reload).
    /// Clears the clones and the key map, deep-clones each domain root, then rebuilds the flat list
    /// from the same clones so selection identity is consistent between the two grids.
    /// </summary>
    public void ProjectReset(IReadOnlyList<FileSystemModel> domainFlat, IReadOnlyList<FileSystemModel> domainTreeRoots)
    {
        DispatcherHelper.RunOnMainThread(() =>
        {
            lock (_projGate)
            {
                try
                {
                    _cloneByKey.Clear();
                    FileTree.Clear();
                    FileList.Clear();
                    _invisibleCloneRoot = null;

                    if (domainTreeRoots.Count > 0)
                    {
                        var first = domainTreeRoots[0];
                        if (first.Parent?.Name == FileSystemModel.ProjectDirName)
                        {
                            var inv = first.Parent;
                            _invisibleCloneRoot = new FileSystemModel(null, FileSystemModel.ProjectDirName, inv.RawRelativePath, true);
                            _cloneByKey[_invisibleCloneRoot.FullName] = _invisibleCloneRoot;
                        }
                    }

                    foreach (var root in domainTreeRoots)
                    {
                        var rootClone = CloneSubtree(root, _invisibleCloneRoot);
                        FileTree.Add(rootClone);

                        if (_invisibleCloneRoot != null && !_invisibleCloneRoot.Children.Contains(rootClone))
                        {
                            _invisibleCloneRoot.Children.Add(rootClone);
                        }
                    }

                    var flatClones = new List<FileSystemModel>(domainFlat.Count);
                    foreach (var d in domainFlat)
                    {
                        if (_cloneByKey.TryGetValue(d.FullName, out var c))
                        {
                            flatClones.Add(c);
                        }
                    }
                    FileList.AddRange(flatClones);
                }
                catch
                {
                    // Projection is best-effort; a file vanishing mid-clone must never crash the UI
                    // thread. A manual Refresh rebuilds cleanly.
                }
            }
        });
    }

    /// <summary>Project one or more newly-added domain nodes (FS create / import) onto the clones.</summary>
    public void ProjectAdd(IReadOnlyList<FileSystemModel> domainNodes)
    {
        if (domainNodes.Count == 0)
        {
            return;
        }

        DispatcherHelper.RunOnMainThread(() =>
        {
            lock (_projGate)
            {
                // Build every clone for the batch first, collecting the NEW ones, then push them onto the
                // grid-bound collections in as few notifications as possible. This is the whole point of
                // the PR: a per-node FileList.Add fires a CollectionChanged each (the grid re-processes
                // every one) and the old per-node FileList.Contains was O(n) → O(n^2) on a 90k-file
                // import. Here dedup is O(1) via _cloneByKey and the grids get one AddRange each.
                var newFlatClones = new List<FileSystemModel>(domainNodes.Count);
                var newTreeRoots = new List<FileSystemModel>();
                var newChildrenByParent =
                    new Dictionary<FileSystemModel, List<FileSystemModel>>(ReferenceEqualityComparer.Instance);

                foreach (var d in domainNodes)
                {
                    try
                    {
                        EnsureCloneBatched(d, newFlatClones, newTreeRoots, newChildrenByParent);
                    }
                    catch
                    {
                        // Best-effort: skip a node that can't be cloned right now (e.g. file vanished).
                    }
                }

                // Wire new children into their parents (one AddRange per touched folder), then the roots
                // and the flat list (one AddRange). Every AddRange raises a single CollectionChanged.
                foreach (var kvp in newChildrenByParent)
                {
                    kvp.Key.Children.AddRange(kvp.Value);
                }

                foreach (var root in newTreeRoots)
                {
                    if (!FileTree.Contains(root))
                    {
                        FileTree.Add(root);
                    }
                }

                if (newFlatClones.Count > 0)
                {
                    FileList.AddRange(newFlatClones);
                }
            }
        });
    }

    /// <summary>Project the removal of a domain node (and its whole subtree) onto the clones.</summary>
    public void ProjectRemove(FileSystemModel domainNode)
    {
        DispatcherHelper.RunOnMainThread(() =>
        {
            lock (_projGate)
            {
                if (_cloneByKey.TryGetValue(domainNode.FullName, out var clone))
                {
                    RemoveCloneSubtree(clone);
                }
            }
        });
    }

    /// <summary>Reflect a file-info change (size etc.) from a domain node onto its clone.</summary>
    public void ProjectUpdateFileInfo(FileSystemModel domainNode)
    {
        DispatcherHelper.RunOnMainThread(() =>
        {
            lock (_projGate)
            {
                try
                {
                    if (_cloneByKey.TryGetValue(domainNode.FullName, out var clone) && !clone.IsDirectory)
                    {
                        clone.UpdateFileInfo();
                    }
                }
                catch
                {
                    // Best-effort; reading a vanished file's size must not crash the UI thread.
                }
            }
        });
    }

    /// <summary>
    /// Reflect a rename onto the clone subtree. <paramref name="oldFullName"/> is the clone's key
    /// before the domain rename (the caller captures it just before mutating the domain model),
    /// since the rename changes FullName and we'd otherwise have no way to find the clone.
    /// </summary>
    public void ProjectRename(FileSystemModel domainNode, string oldFullName)
    {
        DispatcherHelper.RunOnMainThread(() =>
        {
            lock (_projGate)
            {
                if (!_cloneByKey.TryGetValue(oldFullName, out var clone) || clone.Parent is null)
                {
                    // No clone, or it's a tree root (FileSystemModel.Rename requires a parent). Roots
                    // (archive/raw/resources) don't get renamed in practice; a reload would fix it.
                    return;
                }

                try
                {
                    clone.Rename(domainNode.Name); // recomputes FullName/GameRelativePath/Hash and recurses children
                    RekeySubtree(clone);
                }
                catch
                {
                    // Best-effort; a failed in-place rename is reconciled by the next Refresh.
                }
            }
        });
    }

    // ---- projection internals (always called under _projGate, on the UI thread) -----------------

    private FileSystemModel CloneSubtree(FileSystemModel domain, FileSystemModel? cloneParent)
    {
        // Copy the domain's metadata instead of re-stat'ing every file — this runs for the whole project
        // on load/reload, so on a huge project the redundant per-file disk stat is a major cost.
        var clone = new FileSystemModel(domain, cloneParent);
        _cloneByKey[clone.FullName] = clone;

        foreach (var child in domain.Children.ToList())
        {
            clone.Children.Add(CloneSubtree(child, clone));
        }

        return clone;
    }

    private FileSystemModel GetOrCreateInvisibleClone(FileSystemModel domainInvisible)
    {
        if (_invisibleCloneRoot == null)
        {
            _invisibleCloneRoot = new FileSystemModel(null, FileSystemModel.ProjectDirName, domainInvisible.RawRelativePath, true);
            _cloneByKey[_invisibleCloneRoot.FullName] = _invisibleCloneRoot;
        }
        return _invisibleCloneRoot;
    }

    /// <summary>
    /// Ensures a clone exists for <paramref name="domain"/> (creating parent clones as needed), but does
    /// NOT touch FileTree/FileList/Children directly — newly-created clones are collected into
    /// <paramref name="newFlatClones"/>, new visible roots into <paramref name="newTreeRoots"/>, and new
    /// child links into <paramref name="newChildrenByParent"/>, so the caller can apply them in batches
    /// (one AddRange each) rather than one CollectionChanged per node. Returns the (new or existing) clone.
    /// </summary>
    private FileSystemModel EnsureCloneBatched(
        FileSystemModel domain,
        List<FileSystemModel> newFlatClones,
        List<FileSystemModel> newTreeRoots,
        Dictionary<FileSystemModel, List<FileSystemModel>> newChildrenByParent)
    {
        if (_cloneByKey.TryGetValue(domain.FullName, out var existing))
        {
            return existing;
        }

        // A node is a visible tree root when its domain parent is the invisible project root. We still
        // create a (hidden) invisible clone parent so the clone's FullName matches the domain's exactly,
        // keeping _cloneByKey lookups reliable and preventing duplicate clones for the same folder.
        FileSystemModel? cloneParent = null;
        var dp = domain.Parent;
        if (dp is not null && dp.Name == FileSystemModel.ProjectDirName)
        {
            cloneParent = GetOrCreateInvisibleClone(dp);
        }
        else if (dp is not null)
        {
            cloneParent = EnsureCloneBatched(dp, newFlatClones, newTreeRoots, newChildrenByParent);
        }

        // Copy metadata from the domain node instead of re-stat'ing the file (see the clone constructor).
        var clone = new FileSystemModel(domain, cloneParent);
        _cloneByKey[clone.FullName] = clone;

        // The clone is brand new, so it can't already be under cloneParent — collect the link for a single
        // batched Children.AddRange rather than an O(n) Contains + per-item Add.
        if (cloneParent is not null)
        {
            if (!newChildrenByParent.TryGetValue(cloneParent, out var kids))
            {
                kids = [];
                newChildrenByParent[cloneParent] = kids;
            }

            kids.Add(clone);
        }

        var isVisibleRoot = dp is null || dp.Name == FileSystemModel.ProjectDirName;
        if (isVisibleRoot)
        {
            newTreeRoots.Add(clone);
        }

        newFlatClones.Add(clone);
        return clone;
    }

    private void RemoveCloneSubtree(FileSystemModel clone)
    {
        foreach (var child in clone.Children.ToList())
        {
            RemoveCloneSubtree(child);
        }

        _cloneByKey.Remove(clone.FullName);

        if (FileList.Contains(clone))
        {
            FileList.Remove(clone);
        }

        if (clone.Parent is not null)
        {
            if (clone.Parent.Children.Contains(clone))
            {
                clone.Parent.Children.Remove(clone);
            }
        }
        else if (FileTree.Contains(clone))
        {
            FileTree.Remove(clone);
        }
    }

    private void RekeySubtree(FileSystemModel root)
    {
        var subtree = new HashSet<FileSystemModel>();
        Collect(root, subtree);

        // The subtree's FullNames just changed; drop any stale keys pointing at these clones, then
        // re-index them by their current FullName.
        foreach (var staleKey in _cloneByKey.Where(kv => subtree.Contains(kv.Value)).Select(kv => kv.Key).ToList())
        {
            _cloneByKey.Remove(staleKey);
        }

        foreach (var node in subtree)
        {
            _cloneByKey[node.FullName] = node;
        }

        static void Collect(FileSystemModel node, HashSet<FileSystemModel> into)
        {
            into.Add(node);
            foreach (var child in node.Children.ToList())
            {
                Collect(child, into);
            }
        }
    }

    public void Dispose()
    {
        // No native resources; the pure-C# machine needs nothing to release.
        // Retained for API compatibility (tests use `using var guard = new GridGuard()`).
    }
}
