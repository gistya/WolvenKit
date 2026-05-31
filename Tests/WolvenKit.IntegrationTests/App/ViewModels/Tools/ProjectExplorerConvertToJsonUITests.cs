using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HandyControl.Tools.Extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using WolvenKit.App.Models.ProjectManagement.Project;
using WolvenKit.App.Controllers;
using WolvenKit.App.Services;
using WolvenKit.App.ViewModels.Tools;
using WolvenKit.Common;
using WolvenKit.Common.Interfaces;
using WolvenKit.Core.Interfaces;
using WolvenKit.IntegrationTests.Helpers;
using WolvenKit.RED4.Types;
using WolvenKit.Views.Tools;
using Xunit;
using Xunit.Sdk;
using Assert = Xunit.Assert;

namespace WolvenKit.IntegrationTests.App.ViewModels.Tools;

[STATestClass]
public class ProjectExplorerConvertToJsonUITests : IDisposable
{
    private readonly string _tempProjectRoot;
    private readonly Cp77Project _project;
    private IHost? _host;
    private ProjectExplorerViewModel? _projectExplorerVm;
    private IWatcherService? _watcherService;
    private ProjectExplorerView? _projectExplorerView;

    public ProjectExplorerConvertToJsonUITests()
    {
        _tempProjectRoot = Path.Combine(Path.GetTempPath(), "WolvenKit_ConvertUITest_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempProjectRoot);

        _project = new Cp77Project(
            Path.Combine(_tempProjectRoot, "testmod"),
            "ConvertToJsonUITest",
            "ConvertToJsonUITest");

        Directory.CreateDirectory(_project.ModDirectory);
        Directory.CreateDirectory(_project.RawDirectory);
        Directory.CreateDirectory(_project.FileDirectory);
    }

    [StaFact]
    public async Task ConvertToJson_FromArchiveSelection_UpdatesWatcherModels_And_TreeGridNodes_Correctly()
    {
        _host = IntegrationTestHost.Create();
        var services = _host.Services;
        services.UseMicrosoftDependencyResolver();

        var resolver = Locator.CurrentMutable;
        resolver.InitializeSplat();

        var archiveManager = services.GetRequiredService<IArchiveManager>();

        var settingsManager = services.GetRequiredService<ISettingsManager>();
        var gameDir = ResolveGameDirectory();
        var exePath = Path.Combine(gameDir, "bin", "x64", "Cyberpunk2077.exe");
        settingsManager.CP77ExecutablePath = exePath;
        var gameControllerFactory = services.GetRequiredService<IGameControllerFactory>();
        var projectManager = services.GetRequiredService<IProjectManager>();
        var controller = gameControllerFactory.GetRed4Controller();
        Assert.NotNull(controller);

        await controller.L
        await projectManager.LoadAsync(_project.Location);

        var assetBrowserVm = services.GetRequiredService<AssetBrowserViewModel>();
        _projectExplorerVm = services.GetRequiredService<ProjectExplorerViewModel>();
        _watcherService = services.GetRequiredService<IWatcherService>();
        // _projectExplorerView = services.GetRequiredService<IViewFor<ProjectExplorerViewModel>>() as ProjectExplorerView;
        // Assert.NotNull(_projectExplorerView);
        //
        // _projectExplorerView.DataContext = _projectExplorerVm;
        _watcherService.WatchProject(_project);

        await assetBrowserVm.LoadAssetBrowser();
        var folderToPopulate = assetBrowserVm
            ._boundRootNodes.First()
            .Directories["base"]
            .Directories["base\\animations"]
            .Directories["base\\animations\\anim_motion_database"];
        assetBrowserVm.LeftSelectedItem = folderToPopulate;

        var key = archiveManager.GetGameFile(new ResourcePath(folderToPopulate.Name)).Key;
        var archives = archiveManager
            .Archives
            .Items
            .Where(archive => archive.Files.ContainsKey(key));

        foreach (var archive in archives)
        {
            AddFromArchiveItems.Add(archive);
        }


        assetBrowserVm.UpdateSearchInArchives();

        var iGameFiles = folderToAdd.Files;
        List<RedFileViewModel> filesToAdd = [];
        iGameFiles.ToList().ForEach(f => filesToAdd.Add(new RedFileViewModel(f)));

        filesToAdd.ForEach(file => assetBrowserVm.RightItems.Add(file));
        assetBrowserVm.RightItems.ForEach(item => item.IsChecked = true);
        await assetBrowserVm.AddSelectedAsync();
        Assert.True(_projectExplorerVm!.FileList.Count > 5);
    }

    private static string ResolveGameDirectory()
    {
        var dir = Environment.GetEnvironmentVariable("CP77_DIR", EnvironmentVariableTarget.User);
        if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
            return dir;

        throw new XunitException("CP77_DIR user environment variable must point to a valid Cyberpunk 2077 installation.");
    }

    public void Dispose()
    {
        try
        {
            _watcherService?.ForceStop();
            _host?.Dispose();
            if (Directory.Exists(_tempProjectRoot))
                Directory.Delete(_tempProjectRoot, true);
        }
        catch { /* best effort */ }
    }
}
