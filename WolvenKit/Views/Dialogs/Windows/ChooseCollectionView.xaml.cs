using System.Windows;
using WolvenKit.App.ViewModels.Dialogs;

namespace WolvenKit.Views.Dialogs.Windows
{
    /// <summary>
    /// Interaction logic for ChooseCollectionView.xaml
    /// </summary>
    public partial class ChooseCollectionView
    {
        public ChooseCollectionView()
        {
            InitializeComponent();

            ViewModel = WolvenKit.AppImpl.Services?.GetService<ChooseCollectionViewModel>();
            DataContext = ViewModel;


            {
                    vm => vm.AvailableItems,
                    v => v.AvailableDataGrid.ItemsSource)
                    vm => vm.SelectedAvailableItem,
                    v => v.AvailableDataGrid.SelectedItem)
                    vm => vm.SelectedAvailableItems,
                    v => v.AvailableDataGrid.SelectedItems)

                    vm => vm.SelectedItems,
                    v => v.SelectedDataGrid.ItemsSource)
                    vm => vm.SelectedSelectedItem,
                    v => v.SelectedDataGrid.SelectedItem)
                    vm => vm.SelectedSelectedItems,
                    v => v.SelectedDataGrid.SelectedItems)

                    vm => vm.AddItemCommand,
                    v => v.AddButton)
                    vm => vm.RemoveItemCommand,
                    v => v.RemoveButton)

            });

        }

        public ChooseCollectionViewModel ViewModel { get; set; }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }
    }
}
