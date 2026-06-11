using WolvenKit.App.ViewModels.Dialogs;

namespace WolvenKit.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for NewFileView.xaml
    /// </summary>
    public partial class NewFileView : System.Windows.Controls.UserControl
    {
        public NewFileView()
        {
            InitializeComponent();


            {
                    vm => vm.Categories,
                    v => v.Categories.ItemsSource)
                    vm => vm.SelectedCategory,
                    v => v.Categories.SelectedItem)

                    vm => vm.SelectedCategory.Files,
                    v => v.DataGrid.ItemsSource)
                    vm => vm.SelectedFile,
                    v => v.DataGrid.SelectedItem)

                    vm => vm.FileName,
                    v => v.FileName.Text)


            });

        }
    }
}
