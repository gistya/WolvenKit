using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WolvenKit.App.Models.ProjectManagement.Project;
using WolvenKit.App.ViewModels.Dialogs;
using Window = System.Windows.Window;

namespace WolvenKit.Views.Dialogs.Windows
{
    public partial class AddItemsToStoreDialog
    {
        private static string s_lastYamlFile = string.Empty;
        private static string s_lastRedsFile = string.Empty;
        private static readonly List<string> s_lastItemCodes = [];
        private static bool s_rememberValues = false;


        public AddItemsToStoreDialog(Cp77Project project)
        {
            InitializeComponent();

            ViewModel = new AddItemsToStoreDialogViewModel(project, s_rememberValues, s_lastYamlFile, s_lastRedsFile);

            DataContext = ViewModel;

            Owner = Application.Current.MainWindow;

            
        }

        public AddItemsToStoreDialogViewModel ViewModel { get; set; }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }

        private void CloseDialogue(bool result)
        {
            SaveDefaultValues();
            DialogResult = result;
            Close();
        }

        private void WizardControl_OnFinish(object _, RoutedEventArgs e) => SaveDefaultValues();

        private void SaveDefaultValues()
        {
            if (ViewModel is not AddItemsToStoreDialogViewModel vm)
            {
                return;
            }

            s_rememberValues = vm.RememberValues;
            s_lastItemCodes.Clear();

            if (!vm.RememberValues)
            {
                s_lastRedsFile = null;
                s_lastYamlFile = null;
                return;
            }

            s_lastRedsFile = vm.RedsPath;
            s_lastYamlFile = vm.YamlPath;
            s_lastItemCodes.AddRange(vm.ItemCodes);
        }


        [GeneratedRegex(@"Items\.\w+")]
        private partial Regex ItemCodesRegex();

        private void OnWizardKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape)
            {
                return;
            }

            if (ViewModel is { } vm)
            {
                vm.ItemCodes.Clear();
            }

            CloseDialogue(false);
        }

        private void OnTextboxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }

            ViewModel?.Validate();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox textBox || ViewModel is not { } vm)
            {
                return;
            }

            vm.ItemCodes = ItemCodesRegex().Matches(textBox.Text).Select(m => m.Value).Distinct().ToList();
        }

        private void TextBox_MouseLeave(object sender, MouseEventArgs e) => ViewModel?.Validate();

        private void TextBox_FocusLost(object sender, KeyboardFocusChangedEventArgs e) => ViewModel?.Validate();
    }
}
