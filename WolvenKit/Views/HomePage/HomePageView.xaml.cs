using System.Windows;
using System.Windows.Controls;
using WolvenKit.App.Extensions;
using WolvenKit.App.Services;
using WolvenKit.App.ViewModels.HomePage;
using WolvenKit.App.ViewModels.HomePage.Pages;
using WolvenKit.App.ViewModels.Shell;
using WolvenKit.Views.Shell;

namespace WolvenKit.Views.HomePage
{
    public partial class HomePageView
    {
    public HomePageViewModel ViewModel
    {
        get => DataContext as HomePageViewModel;
        set => DataContext = value;
    }

        public HomePageView()
        {
            InitializeComponent();


        }

        private void Grid_MouseLeftButtonDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // dumb hack
            if (ViewModel.SelectedIndex == (int)EHomePage.Mods)
            {
                return;
            }

            if (!e.Handled)
            {
                var mainWindow = System.Windows.Application.Current?.MainWindow as MainView;
                mainWindow?.DragMove();
            }
        }

        private void Grid_MouseLeftButtonDown_2(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2)
            {
                base.OnMouseLeftButtonDown(e);
                var mainWindow = System.Windows.Application.Current?.MainWindow as MainView;
                mainWindow?.DragMove();
            }
        }

        // TODO: Can't you bind this directly in XAML? -wopss

        private void ModsPageTab_Selected(object sender, RoutedEventArgs e)
        {
            if (sender is TabItem tab && tab.Content is Pages.ModsView view && view.DataContext is ModsViewModel vm)
            {
                vm.CheckRedModCommand.SafeExecute();
            }
        }

        //private void PluginsPageTab_Selected(object sender, RoutedEventArgs e)
        //{
        //    if (sender is TabItem tab && tab.Content is Pages.PluginsToolView view && view.DataContext is PluginsToolViewModel vm)
        //    {
        //        //vm.ReloadCommand.SafeExecute();
        //    }
        //}
    }
}
