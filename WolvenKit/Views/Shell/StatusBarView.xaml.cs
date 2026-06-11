using System;
using Microsoft.Extensions.DependencyInjection;
using WolvenKit.App.ViewModels.Shell;

namespace WolvenKit.Views.Shell
{
    public partial class StatusBarView : System.Windows.Controls.UserControl
    {
        #region Constructors

        public StatusBarView()
        {
            InitializeComponent();

            ViewModel = WolvenKit.AppImpl.Services?.GetService<StatusBarViewModel>();
            DataContext = ViewModel;

            // Bindings for progress moved to XAML where possible (StatusBarProgressBar.Progress / IsIndeterminate)
        }

        #endregion Constructors

        private void StatusBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!e.Handled)
            {
                // main window drag - reference self's owner or Application
                var mainWindow = System.Windows.Application.Current?.MainWindow as MainView;
                mainWindow?.DragMove();
            }
        }
    }
}
