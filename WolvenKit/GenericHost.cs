using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using WolvenKit.App;
using WolvenKit.App.Controllers;
using WolvenKit.App.Factories;
using WolvenKit.App.Helpers;
using WolvenKit.App.Models.ProjectManagement;
using WolvenKit.App.Services;
using WolvenKit.App.ViewModels.Dialogs;
using WolvenKit.App.ViewModels.Exporters;
using WolvenKit.App.ViewModels.HomePage;
using WolvenKit.App.ViewModels.HomePage.Pages;
using WolvenKit.App.ViewModels.Importers;
using WolvenKit.App.ViewModels.Shell;
using WolvenKit.App.ViewModels.Tools;
using WolvenKit.Common;
using WolvenKit.Common.Interfaces;
using WolvenKit.Common.Services;
using WolvenKit.Core.Interfaces;
using WolvenKit.Core.Services;
using WolvenKit.Modkit.RED4;
using WolvenKit.Modkit.RED4.Tools;
using WolvenKit.RED4.CR2W;
using WolvenKit.Services;
using WolvenKit.ViewModels;
using WolvenKit.Views.Dialogs;
using WolvenKit.Views.Dialogs.Windows;
using WolvenKit.Views.Exporters;
using WolvenKit.Views.HomePage;
using WolvenKit.Views.HomePage.Pages;
using WolvenKit.Views.Importers;
using WolvenKit.Views.Shell;
using WolvenKit.Views.Tools;

namespace WolvenKit
{
    public static class GenericHost
    {
        public static IHostBuilder CreateHostBuilder() => Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    var assemblyFolder = Path.GetDirectoryName(System.AppContext.BaseDirectory);

                    configuration.SetBasePath(assemblyFolder);
                    configuration.AddJsonFile("appsettings.json");
                })
                .ConfigureServices(services =>
                {
                    services.UseMicrosoftDependencyResolver();
                    var resolver = Locator.CurrentMutable;
                    resolver.InitializeSplat();
                    // ReactiveUI view locator removed; using standard DI + DataTemplates for view resolution
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // services
                    services.AddSingleton(typeof(ISettingsManager), SettingsManager.Load());
                    services.AddSingleton<IHashService, HashServiceExt>();                                      // can this be transient?
                    services.AddSingleton<CRUIDService>(x => new CRUIDService(false));    // can this be transient?
                    services.AddSingleton<MySink>();                                                            // can this be transient?
                    services.AddSingleton<ILoggerService, SerilogWrapper>();                                    // can this be transient?
                    services.AddSingleton<ITweakDBService, TweakDBService>();
                    services.AddSingleton<IUpdateService, UpdateService>();

                    services.AddSingleton<ArchiveXlItemService>();
                    services.AddSingleton<ICvmTools, CvmTools>();

                    // scripting
                    services.AddSingleton<IHookService, AppHookService>();
                    services.AddSingleton<AppScriptService>();
                    services.AddTransient<ImportExportHelper>();

                    services.AddTransient<INotificationService, NotificationService>();
                    services.AddSingleton<IProgressService<double>, ProgressService<double>>();
                    services.AddSingleton<AppIdleStateService>();
                    services.AddTransient<Red4ParserService>();
                    services.AddSingleton<IAppArchiveManager, AppArchiveManager>();
                    services.AddSingleton<IArchiveManager>(provider => provider.GetService<IAppArchiveManager>());
                    services.AddTransient<ILocKeyService, LocKeyServiceExt>();                  // can this be transient?
                    services.AddSingleton<IRecentlyUsedItemsService, RecentlyUsedItemsService>();
                    services.AddSingleton<IProjectManager, ProjectManager>();
                    services.AddTransient<GeometryCacheService>();
                    services.AddTransient<MeshTools>();
                    services.AddTransient<IModTools, ModTools>();
                    services.AddTransient<MockGameController>();
                    services.AddTransient<RED4Controller>();
                    services.AddTransient<IGameControllerFactory, GameControllerFactory>();
                    services.AddSingleton<IPluginService, PluginService>();
                    services.AddSingleton<IModifierViewStateService, ModifierViewStateService>();
                    services.AddSingleton<INodeSelectionService, NodeSelectionService>();
                    services.AddSingleton<IProjectEvents, ProjectEvents>();

                    // factories
                    services.AddTransient<IPageViewModelFactory, PageViewModelFactory>();
                    services.AddTransient<IDialogViewModelFactory, DialogViewModelFactory>();
                    services.AddTransient<IDocumentTabViewmodelFactory, DocumentTabViewmodelFactory>();
                    services.AddTransient<IChunkViewmodelFactory, ChunkViewmodelFactory>();             // IDocumentTabViewmodelFactory
                    services.AddTransient<IPaneViewModelFactory, PaneViewModelFactory>();               // IChunkViewmodelFactory
                    services.AddTransient<INodeWrapperFactory, NodeWrapperFactory>();
                    services.AddTransient<IDocumentViewmodelFactory, DocumentViewmodelFactory>();       //IDocumentTabViewmodelFactory, IPaneViewModelFactory, IChunkViewmodelFactory

                    // register viewmodels (views are resolved via DataTemplates, direct new, or AddTransient for root views)
                    #region shell

                    services.AddSingleton<AppViewModel>();
                    services.AddTransient<MainView>();

                    services.AddTransient<RibbonViewModel>();
                    services.AddTransient<RibbonView>();

                    services.AddTransient<MenuBarViewModel>();
                    services.AddTransient<MenuBarView>();

                    services.AddTransient<StatusBarViewModel>();
                    services.AddTransient<StatusBarView>();

                    #endregion

                    #region dialogs

                    services.AddTransient<InputDialogViewModel>();
                    services.AddTransient<InputDialogView>();

                    services.AddTransient<SearchAndReplaceDialogViewModel>();
                    services.AddTransient<SearchAndReplaceDialog>();

                    services.AddTransient<CreateMaterialsDialogViewModel>();
                    services.AddTransient<CreateMaterialsDialog>();

                    services.AddTransient<RenameDialogViewModel>();
                    services.AddTransient<RenameDialog>();

                    services.AddTransient<SaveGameSelectionDialogModel>();
                    services.AddTransient<SaveGameSelectionDialog>();

                    services.AddTransient<LaunchProfilesViewModel>();
                    services.AddTransient<LaunchProfilesView>();

                    services.AddTransient<MaterialsRepositoryViewModel>();
                    services.AddTransient<MaterialsRepositoryView>();

                    services.AddTransient<NewFileViewModel>();
                    services.AddTransient<NewFileView>();

                    services.AddTransient<SoundModdingViewModel>();
                    services.AddTransient<SoundModdingView>();

                    services.AddTransient<FirstSetupViewModel>();
                    services.AddTransient<FirstSetupView>();

                    services.AddTransient<ProjectWizardViewModel>();
                    services.AddTransient<ProjectWizardView>();

                    services.AddTransient<ChooseCollectionViewModel>();
                    services.AddTransient<ChooseCollectionView>();


                    // Importers

                    services.AddTransient<ImportViewModel>();
                    services.AddTransient<ImportView>();

                    services.AddTransient<ExportViewModel>();
                    services.AddTransient<ExportView>();

                    #endregion

                    #region documents / tools (for direct resolve + DataTemplates still create via parameterless ctor)

                    services.AddTransient<AssetBrowserViewModel>();
                    services.AddTransient<AssetBrowserView>();

                    services.AddTransient<LogViewModel>();
                    services.AddTransient<LogView>();

                    services.AddSingleton<ProjectExplorerViewModel>();
                    services.AddTransient<ProjectExplorerView>();

                    services.AddSingleton<PropertiesViewModel>();
                    services.AddTransient<PropertiesView>();

                    services.AddTransient<TweakBrowserViewModel>();
                    services.AddTransient<TweakBrowserView>();

                    services.AddTransient<LocKeyBrowserViewModel>();
                    services.AddTransient<LocKeyBrowserView>();

                    #endregion

                    #region tools

                    services.AddTransient<AudioPlayerViewModel>();
                    services.AddTransient<AudioPlayerView>();

                    services.AddTransient<HashToolViewModel>();
                    services.AddTransient<HashToolView>();

                    services.AddSingleton<DocumentTools>();
                    services.AddSingleton<TemplateFileTools>();
                    services.AddSingleton<Cr2WTools>();
                    services.AddSingleton<ProjectResourceTools>();
                    #endregion

                    #region homepage

                    services.AddTransient<HomePageViewModel>();
                    services.AddTransient<HomePageView>();

                    services.AddTransient<SettingsPageViewModel>();
                    services.AddTransient<SettingsPageView>();

                    services.AddTransient<WelcomePageViewModel>();
                    services.AddTransient<WelcomePageView>();

                    services.AddTransient<ModsViewModel>();
                    services.AddTransient<ModsView>();

                    services.AddTransient<PluginsToolViewModel>();
                    services.AddTransient<PluginsToolView>();

                    #endregion

                    // bind options
                    services.AddOptions<Globals>().Bind(hostContext.Configuration.GetSection("Globals"));

                })
                .UseEnvironment(Environments.Development);
    }
}
