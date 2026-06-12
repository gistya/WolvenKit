using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using WolvenKit.App.Services;
using WolvenKit.App.ViewModels.Shell;
using WolvenKit.Core.Interfaces;
using WolvenKit.Views.Templates;

namespace WolvenKit.Views.Shell
{
    public partial class RibbonView : System.Windows.Controls.UserControl
    {
        public RibbonViewModel ViewModel
        {
            get => DataContext as RibbonViewModel;
            set => DataContext = value;
        }

        private readonly ISettingsManager _settingsManager;
        private readonly ILoggerService _loggerService;

        public const string LaunchProfilesString = "Launch Profiles";

        public RibbonView()
        {
            ViewModel = WolvenKit.AppImpl.Services?.GetService<RibbonViewModel>();
            DataContext = ViewModel;
            InitializeComponent();

            _settingsManager = WolvenKit.AppImpl.Services?.GetService<ISettingsManager>();
            _loggerService = WolvenKit.AppImpl.Services?.GetService<ILoggerService>();

            // Reactive Bind* removed. Commands are now bound in XAML (or wired below for complex cases).
            // For LaunchMenu.IsEnabled and LaunchProfileText we use a mix of XAML + code for minimal diff.

            if (ViewModel is not null && !string.IsNullOrEmpty(_settingsManager?.LastLaunchProfile))
            {
                ViewModel.LaunchProfileText = _settingsManager.LastLaunchProfile;
            }

            if (_settingsManager != null)
            {
                // Note: WhenAnyValue removed; if launch profiles change we could subscribe to PropertyChanged on settings
                // For now call once; the menu population also happens on click paths.
                _settingsManager.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(ISettingsManager.LaunchProfiles))
                    {
                        GetLaunchProfiles();
                    }
                };
            }

            GetLaunchProfiles();
        }


        private void GetLaunchProfiles()
        {
            // unsubscribe
            foreach (var obj in LaunchMenuMainItem.Items)
            {
                if (obj is not MenuItem { Header: string menuitemHeader } menuitem || menuitemHeader == LaunchProfilesString)
                {
                    continue;
                }

                menuitem.Click -= LaunchMenu_MenuItem_Click;
            }

            // delete all except for last two (separator and "Launch options")
            var cntToRemove = LaunchMenuMainItem.Items.Count - 2;
            for (var i = 0; i < cntToRemove; i++)
            {
                LaunchMenuMainItem.Items.RemoveAt(0);
            }

            var count = 0;
            foreach (var (name, _) in _settingsManager.LaunchProfiles)
            {
                MenuItem item = new()
                {
                    Header = name,
                    Icon = new IconBox()
                };

                item.Click += LaunchMenu_MenuItem_Click;

                LaunchMenuMainItem.Items.Insert(count, item);
                count++;
            }

            if (ViewModel is not null && ViewModel.LaunchProfileText is null && _settingsManager.LaunchProfiles.Count > 0)
            {
                ViewModel.LaunchProfileText = _settingsManager.LaunchProfiles.First().Key;
            }
        }

        private void LaunchMenu_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel is null || e.Source is not MenuItem { Header: string header })
            {
                return;
            }

            ViewModel.LaunchProfileText = header;
            _settingsManager.LastLaunchProfile = header;
        }
    }
}
