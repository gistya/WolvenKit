using WolvenKit.App.ViewModels.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace WolvenKit.Views.Dialogs.Windows
{
    public partial class FirstSetupView
    {
        public FirstSetupView()
        {
            InitializeComponent();

            ViewModel = WolvenKit.AppImpl.Services?.GetService<FirstSetupViewModel>();
            DataContext = ViewModel;

            {
                WizardControl.Finish += (sender, args) =>
                {
                    ViewModel.ExecuteFinish();
                };
            }

        }

        public FirstSetupViewModel ViewModel { get; set; }

        private void Field_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => ValidateAllFields();

        private void ValidateAllFields() => ViewModel.AllFieldsValid = !ViewModel.HasErrors;
    }
}
