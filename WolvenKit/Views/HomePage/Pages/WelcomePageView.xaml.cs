using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using WolvenKit.App.ViewModels.HomePage.Pages;
using WolvenKit.Functionality.Helpers;

namespace WolvenKit.Views.HomePage.Pages
{
    public partial class WelcomePageView : System.Windows.Controls.UserControl
    {
        public WelcomePageView()
        {
            InitializeComponent();

            ViewModel = WolvenKit.AppImpl.Services?.GetService<WelcomePageViewModel>();
            DataContext = ViewModel;

            // Bindings for filters, orders, and commands are now declared directly in WelcomePageView.xaml
            // (converted from previous ReactiveUI WhenActivated / Bind calls).
        }
    }
}
