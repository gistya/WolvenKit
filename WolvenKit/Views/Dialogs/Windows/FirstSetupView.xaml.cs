using WolvenKit.App.ViewModels.Dialogs;

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

                        vm => vm.AllFieldsValid,
                        v => v.WizardControl.FinishEnabled)

                    vm => vm.OpenCP77GamePathCommand,

                    vm => vm.OpenDepotPathCommand,

                    ViewModel,
                    vm => vm.OpenLinkCommand,
                    v => v.helpButton,
                    vm => vm.WikiHelpLink);
            });

        }

        public FirstSetupViewModel ViewModel { get; set; }

        private void Field_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => ValidateAllFields();

        private void ValidateAllFields() => ViewModel.AllFieldsValid = !ViewModel.HasErrors;
    }
}
