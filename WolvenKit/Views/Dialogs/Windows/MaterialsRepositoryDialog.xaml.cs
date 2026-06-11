using System;
using System.Windows;
using WolvenKit.App.Helpers;
using WolvenKit.App.Services;
using WolvenKit.App.ViewModels.Dialogs;

namespace WolvenKit.Views.Dialogs.Windows
{
    /// <summary>
    /// Interaction logic for MaterialsRepositoryDialog.xaml
    /// </summary>
    public partial class MaterialsRepositoryView
    {

        public MaterialsRepositoryView()
        {
            InitializeComponent();

            ViewModel = WolvenKit.AppImpl.Services?.GetService<MaterialsRepositoryViewModel>();
            DataContext = ViewModel;

            {


            });
        }

        public MaterialsRepositoryViewModel ViewModel { get; set; }

        private void MaterialsButton_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dialog = new FolderPicker();

            if (dialog.ShowDialog() == true)
            {
                ViewModel.MaterialsDepotPath = dialog.ResultPath;

                var settingsManager = WolvenKit.AppImpl.Services?.GetService<ISettingsManager>();
                settingsManager.MaterialRepositoryPath = dialog.ResultPath;
                settingsManager.Save();
            }
        }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }
    }
}
