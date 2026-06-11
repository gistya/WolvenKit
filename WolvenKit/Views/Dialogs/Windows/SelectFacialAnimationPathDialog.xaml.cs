using System.Collections.Generic;
using System.Windows.Input;
using WolvenKit.App.ViewModels.Dialogs;
using Window = System.Windows.Window;

namespace WolvenKit.Views.Dialogs.Windows
{
    public partial class SelectFacialAnimationPathDialog
    {
        public SelectFacialAnimationPathDialog(List<string> facialSetupPaths)
        {
            InitializeComponent();
            
            ViewModel = new SelectAnimationPathViewModel(facialSetupPaths);
            DataContext = ViewModel;

            {
                // bind to filteredDropdownMenu
                        x => x.AnimGraphOptions,
                        x => x.FilterableDropdownMenu.Options)
                        x => x.SelectedGraph,
                        x => x.FilterableDropdownMenu.SelectedOption)
            });
        }

        public SelectAnimationPathViewModel ViewModel { get; set; }

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