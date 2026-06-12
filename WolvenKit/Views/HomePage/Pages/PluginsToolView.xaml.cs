using WolvenKit.App.ViewModels.HomePage.Pages;
using Microsoft.Extensions.DependencyInjection;


namespace WolvenKit.Views.HomePage.Pages
{
    /// <summary>
    /// Interaction logic for PluginsToolView.xaml
    /// </summary>
    public partial class PluginsToolView : System.Windows.Controls.UserControl
    {
        public PluginsToolViewModel ViewModel
        {
            get => DataContext as PluginsToolViewModel;
            set => DataContext = value;
        }

        public PluginsToolView()
        {
            InitializeComponent();

            ViewModel = WolvenKit.AppImpl.Services?.GetService<PluginsToolViewModel>();
            DataContext = ViewModel;

        }
    }
}
