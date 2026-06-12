using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using WolvenKit.App.Models.ProjectManagement.Project;
using WolvenKit.App.ViewModels.Dialogs;

namespace WolvenKit.Views.Dialogs
{
    public partial class FolderPathInputDialogView
    {
        public FolderPathInputDialogView(Cp77Project activeProject, string dialogTitle = "")
        {
            DialogTitle = dialogTitle;
            
            InitializeComponent();
            Loaded += (s, e) => TextBox.Focus();

            ViewModel = new FolderPathInputDialogViewModel(activeProject, dialogTitle);
            DataContext = ViewModel;

        }

        public string DialogTitle;
        public FolderPathInputDialogViewModel ViewModel { get; set; }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }

        private void WizardPage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is not (Key.Enter or Key.Escape))
            {
                return;
            }

            e.Handled = true;
            DialogResult = e.Key == Key.Enter;
            Close();
        }

        private void Textbox_OnKeyUp(object sender, KeyEventArgs e) => ViewModel?.RefreshValiditiy();
    }
}