using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using WolvenKit.App.ViewModels.Dialogs;

namespace WolvenKit.Views.Dialogs
{
    public partial class InputDialogView
    {
        public InputDialogView(string dialogTitle = "", string defaultValue = "")
        {
            DialogTitle = dialogTitle;
            
            InitializeComponent();
            Loaded += (s, e) => TextBox.Focus();

            ViewModel = new InputDialogViewModel(dialogTitle, defaultValue);
            DataContext = ViewModel;

            {
                        x => x.Text,
                        x => x.TextBox.Text)

                if (defaultValue != "")
                {
                    TextBox.SelectAll();
                }
            });
        }

        public string DialogTitle;

        public InputDialogViewModel ViewModel { get; set; }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }

        private void WizardPage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            e.Handled = true;
            DialogResult = true;
            Close();
        }
    }
}