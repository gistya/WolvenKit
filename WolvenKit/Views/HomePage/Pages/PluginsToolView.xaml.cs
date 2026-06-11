using WolvenKit.App.ViewModels.HomePage.Pages;


namespace WolvenKit.Views.HomePage.Pages
{
    /// <summary>
    /// Interaction logic for PluginsToolView.xaml
    /// </summary>
    public partial class PluginsToolView : System.Windows.Controls.UserControl
    {
        public PluginsToolView()
        {
            InitializeComponent();

            ViewModel = WolvenKit.AppImpl.Services?.GetService<PluginsToolViewModel>();
            DataContext = ViewModel;

                    viewModel => viewModel.SyncCommand,
                    view => view.CheckButton));
        }
    }
}
