using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WolvenKit.App.Interaction;
using WolvenKit.App.Interaction.Options;
using WolvenKit.App.Models.ProjectManagement.Project;
using WolvenKit.App.ViewModels.Dialogs;
using WolvenKit.App.ViewModels.Shell;

namespace WolvenKit.Views.Shell;

/// <summary>
/// Interaction logic for MenuBarView.xaml
/// </summary>
public partial class MenuBarView : System.Windows.Controls.UserControl
{
        public MenuBarViewModel ViewModel
        {
            get => DataContext as MenuBarViewModel;
            set => DataContext = value;
        }

    private AppViewModel _mainViewModel;

    //public static MaterialsRepositoryDialog MaterialsRepositoryDia { get; set; }

    public MenuBarView()
    {
        ViewModel = WolvenKit.AppImpl.Services?.GetService<MenuBarViewModel>();
        DataContext = ViewModel;

        InitializeComponent();

        _mainViewModel = ViewModel?.MainViewModel ?? WolvenKit.AppImpl.Services?.GetService<AppViewModel>();

        // This preserves exact previous behavior without requiring XAML changes to every MenuItem.
        if (_mainViewModel != null)
        {
            // Home
            if (HomeButton != null) HomeButton.Command = _mainViewModel.ShowHomePageCommand;

            // File
            if (MenuItemNewFile != null) MenuItemNewFile.Command = _mainViewModel.NewFileCommand;
            if (MenuItemNewPhotoModeFiles != null) MenuItemNewPhotoModeFiles.Command = _mainViewModel.NewPhotoModeFilesCommand;
            if (MenuItemGenerateInkatlas != null) MenuItemGenerateInkatlas.Command = _mainViewModel.GenerateInkatlasCommand;
            if (MenuItemGenerateMinimalQuest != null) MenuItemGenerateMinimalQuest.Command = _mainViewModel.GenerateMinimalQuestFilesCommand;
            if (MenuItemAddOrEditRadio != null) MenuItemAddOrEditRadio.Command = _mainViewModel.AddOrEditRadioCommand;
            if (MenuItemRegisterWorldbuilderFiles != null) MenuItemRegisterWorldbuilderFiles.Command = _mainViewModel.RegisterWorldbuilderFilesCommand;
            if (MenuItemGeneratePropFile != null) MenuItemGeneratePropFile.Command = _mainViewModel.GeneratePropItemCommand;
            if (MenuItemAddPlayerHead != null) MenuItemAddPlayerHead.Command = _mainViewModel.AddPlayerHeadCommand;

            // Archive
            if (MenuItemImportArchive != null) MenuItemImportArchive.Command = _mainViewModel.ImportArchiveCommand;
            if (MenuItemAddAxlItemFiles != null) MenuItemAddAxlItemFiles.Command = _mainViewModel.AddAXlItemFilesCommand;
            if (MenuItemSave != null) MenuItemSave.Command = _mainViewModel.SaveFileCommand;
            if (MenuItemSaveAs != null) MenuItemSaveAs.Command = _mainViewModel.SaveAsCommand;
            if (MenuItemSaveAll != null) MenuItemSaveAll.Command = _mainViewModel.SaveAllCommand;

            // Project
            if (MenuItemNewProject != null) MenuItemNewProject.Command = _mainViewModel.NewProjectCommand;
            if (MenuItemOpenProject != null) MenuItemOpenProject.Command = _mainViewModel.OpenProjectCommand;

            // Edit / Project tools
            if (ToolbarSettingsButton != null) ToolbarSettingsButton.Command = _mainViewModel.ShowSettingsCommand;
            if (ToolbarProjectSettingsButton != null) ToolbarProjectSettingsButton.Command = _mainViewModel.ShowProjectSettingsCommand;
            if (ToolbarProjectScanFilePathsButton != null) ToolbarProjectScanFilePathsButton.Command = _mainViewModel.ScanForBrokenReferencePathsCommand;
            if (ToolbarProjectScanForBrokenFilesButton != null) ToolbarProjectScanForBrokenFilesButton.Command = _mainViewModel.ScanForBrokenFilesCommand;
            if (ToolbarProjectFindUnusedFilesButton != null) ToolbarProjectFindUnusedFilesButton.Command = _mainViewModel.FindUnusedFilesCommand;
            if (ToolbarProjectDeleteEmptyFoldersButton != null) ToolbarProjectDeleteEmptyFoldersButton.Command = _mainViewModel.DeleteEmptyFoldersCommand;
            if (ToolbarProjectDeleteEmptyMeshesButton != null) ToolbarProjectDeleteEmptyMeshesButton.Command = _mainViewModel.DeleteEmptyMeshesCommand;
            if (ToolbarProjectRunFileValidationButton != null) ToolbarProjectRunFileValidationButton.Command = _mainViewModel.RunFileValidationOnProjectCommand;
            if (ToolbarImportEntitySpawnerButton != null) ToolbarImportEntitySpawnerButton.Command = _mainViewModel.ImportFromEntitySpawnerCommand;
            if (ToolbarOpenLogsButton != null) ToolbarOpenLogsButton.Command = _mainViewModel.OpenLogsCommand;

            // Build
            // Pack

            // Install

            // Launch

            // Clean All

            // Hot Reload

            // Launch Profiles

            // View

            // Tools

            // Importers
            if (MenuItemShowTextureExporter != null) MenuItemShowTextureExporter.Command = _mainViewModel.ShowTextureExporterCommand;
            if (MenuItemShowHashTool != null) MenuItemShowHashTool.Command = _mainViewModel.ShowHashToolCommand;

            // Game
            if (ToolbarLaunchButton != null) ToolbarLaunchButton.Command = _mainViewModel.LaunchGameCommand;
            if (ToolbarLaunchSteamButton != null) ToolbarLaunchSteamButton.Command = _mainViewModel.LaunchGameCommand;

            // Extensions
            if (MenuItemShowPluginTool != null) MenuItemShowPluginTool.Command = _mainViewModel.ShowPluginCommand;
            if (MenuItemShowModsView != null) MenuItemShowModsView.Command = _mainViewModel.ShowModsViewCommand;

            // visibility checkboxes
            if (ProjectExplorerCheckbox != null) ProjectExplorerCheckbox.IsChecked = ViewModel?.ProjectExplorerCheckbox ?? false;
            if (AssetBrowserCheckbox != null) AssetBrowserCheckbox.IsChecked = ViewModel?.AssetBrowserCheckbox ?? false;
            if (PropertiesCheckbox != null) PropertiesCheckbox.IsChecked = ViewModel?.PropertiesCheckbox ?? false;
            if (LogCheckbox != null) LogCheckbox.IsChecked = ViewModel?.LogCheckbox ?? false;
            if (TweakBrowserCheckbox != null) TweakBrowserCheckbox.IsChecked = ViewModel?.TweakBrowserCheckbox ?? false;
            if (LocKeyBrowserCheckbox != null) LocKeyBrowserCheckbox.IsChecked = ViewModel?.LocKeyBrowserCheckbox ?? false;
        }
    }

    private void SetLayoutToDefault(object sender, RoutedEventArgs e) => DockingAdapter.G_Dock.LoadDefaultLayout();
    private void SaveLayoutToProject(object sender, RoutedEventArgs e) => DockingAdapter.G_Dock.SaveLayout();

    private void ResetDefaultLayout(object sender, RoutedEventArgs e) => DockingAdapter.G_Dock.ResetDefaultLayout();

    private void SaveCurrentLayoutToDefault(object sender, RoutedEventArgs e) => DockingAdapter.G_Dock.SaveLayout(true);
    private void GenerateMaterialRepoButton_Click(object sender, RoutedEventArgs e) => Interactions.ShowMaterialRepositoryView();

    private void GenerateItemCodes_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel is not MenuBarViewModel vm)
        {
            return;
        }

        var itemCodes = vm.GenerateItemCodesFromYaml();

        if (itemCodes.Count == 0)
        {
            return;
        }

        Interactions.ShowDictionaryAsCopyableList(
            new ShowDictAsCopyableListDialogOptions("Item codes:", "These are your item codes", itemCodes));
    }


    private void AddItemsToVendor_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel is not MenuBarViewModel vm || vm.MainViewModel.ActiveProject is not Cp77Project project)
        {
            return;
        }

        var dialogVm = Interactions.AddItemsToStore(project);

        vm.AddItemCodesToFiles(dialogVm);
    }


}
