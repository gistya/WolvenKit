using System.Windows;
using WolvenKit.App.ViewModels.Dialogs;
using Microsoft.Extensions.DependencyInjection;

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


            

        }

        public ChooseCollectionViewModel ViewModel { get; set; }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }
    }
}
