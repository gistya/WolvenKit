using System.Windows;
using System.Windows.Controls;
using WolvenKit.App.ViewModels.Dialogs;

namespace WolvenKit.Views.Dialogs
{
    public partial class ProjectWizardView : System.Windows.Controls.UserControl
    {
        private bool _syncModName = true;
        private bool _autoUpdate = false;

        public ProjectWizardView()
        {
            InitializeComponent();

            {
                        vm => vm.Author,
                        vm => vm.Email,
                        vm => vm.Version,

                    vm => vm.OpenProjectPathCommand,

                    x => x.OkCommand,

                    x => x.CancelCommand,

                if (ViewModel is null)
                {
                    return;
                }

                _syncModName = true;
                _autoUpdate = false;

                ViewModel.ValidateProjectName();
                ViewModel.ValidateProjectPath();
                ViewModel.ValidateModName();

                ViewModel.ReadDefaultValuesFromSettings();
                VersionTextBox.SetCurrentValue(TextBox.TextProperty, "1.0.0");
                AuthorTextBox.SetCurrentValue(TextBox.TextProperty, ViewModel.Author);
                EmailTextBox.SetCurrentValue(TextBox.TextProperty, ViewModel.Email);
            });
        }

        private void ProjectNameTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_syncModName)
            {
                return;
            }

            _autoUpdate = true;
            ModNameTextBox.SetCurrentValue(TextBox.TextProperty, ProjectNameTextBox.Text);
            _autoUpdate = false;
        }

        private void ModNameTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_autoUpdate)
            {
                _syncModName = false;
            }
        }

        private void Author_OnFocusLost(object sender, RoutedEventArgs e) => ViewModel?.SaveAuthorToSettingsIfNeeded();
    }
}
