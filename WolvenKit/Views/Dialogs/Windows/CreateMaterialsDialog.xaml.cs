using System.Windows;
using System.Windows.Input;
using WolvenKit.App.ViewModels.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace WolvenKit.Views.Dialogs.Windows
{
    public partial class CreateMaterialsDialog
    {
        private static string s_lastMaterial = "";
        private static bool? s_lastResolve;
        private static bool? s_lastLocal;

        public CreateMaterialsDialog()
        {
            InitializeComponent();


            ViewModel = WolvenKit.AppImpl.Services?.GetService<CreateMaterialsDialogViewModel>();
            DataContext = ViewModel;

            LoadLastSelection();

        }

        public CreateMaterialsDialogViewModel ViewModel { get; set; }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }

        private void LoadLastSelection()
        {
            if (ViewModel is null)
            {
                return;
            }

            if (s_lastMaterial != "")
            {
                ViewModel.BaseMaterial = s_lastMaterial;
                ViewModel.RememberValues = true;
            }

            if (s_lastLocal is not null)
            {
                ViewModel.IsLocalMaterial = true;
                ViewModel.RememberValues = true;
            }

            if (s_lastResolve is not null)
            {
                ViewModel.ResolveSubstitutions = true;
                ViewModel.RememberValues = true;
            }
        }

        private void SaveLastSelection()
        {
            if (ViewModel is null)
            {
                return;
            }

            if (!ViewModel.RememberValues)
            {
                s_lastMaterial = "";
                s_lastLocal = null;
                s_lastResolve = null;
                return;
            }

            s_lastMaterial = ViewModel.BaseMaterial;
            s_lastLocal = ViewModel.IsLocalMaterial;
            s_lastResolve = ViewModel.ResolveSubstitutions;
        }

        private void WizardPage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            SaveLastSelection();
            e.Handled = true;
            DialogResult = true;
            Close();
        }

        private void WizardControl_OnFinish(object sender, RoutedEventArgs e) => SaveLastSelection();
    }
}