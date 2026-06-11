using System.Linq;
using Syncfusion.UI.Xaml.Grid;
using WolvenKit.App.Extensions;
using WolvenKit.App.ViewModels.HomePage.Pages;

namespace WolvenKit.Views.HomePage.Pages
{
    /// <summary>
    /// Interaction logic for ModsView.xaml
    /// </summary>
    public partial class ModsView : System.Windows.Controls.UserControl
    {
        public ModsView()
        {
            InitializeComponent();

            ViewModel = WolvenKit.AppImpl.Services?.GetService<ModsViewModel>();
            DataContext = ViewModel;

            DataGridEvents.RowDragDropController.Dropped += RowDragDropController_Dropped;
            DataGridEvents.RowDragDropController.DragStart += RowDragDropController_DragStart;

            DataGridEvents.RowDragDropController.Drop += RowDragDropController_Drop;
            // DataGridEvents.RowDragDropController.DragOver += RowDragDropController_DragOver;
            DataGridEvents.Drop += DataGridEvents_Drop;
        }

        private void RowDragDropController_Drop(object sender, GridRowDropEventArgs e)
        {
        }

        private void DataGridEvents_Drop(object sender, System.Windows.DragEventArgs e)
        {
            // REDMODTODO install mod
        }

        private void RowDragDropController_DragStart(object sender, GridRowDragStartEventArgs e)
        {
            var sortDesc = DataGridEvents.SortColumnDescriptions;
            if (sortDesc.Any())
            {
                var sorter = sortDesc.First().ColumnName;
                if (sorter != "LoadOrder")
                {
                    e.Handled = true;
                }
            }
        }

        private void RowDragDropController_Dropped(object sender, GridRowDroppedEventArgs e)
        {
            if (e.IsFromOutSideSource)
            {
                // install mod



            }
            else
            {
                // adjust load order
                var records = DataGridEvents.View.Records.ToList();
                for (var i = 0; i < records.Count; i++)
                {
                    var r = records[i];
                    var idx = GridIndexResolver.ResolveToRowIndex(DataGridEvents, r) - 1;
                    if (r.Data is ModInfoViewModel m)
                    {
                        m.LoadOrder = idx;
                    }
                }

                ViewModel.SetLoadOrderChanged(true);
            }
        }

        private void RemoveModMenuItem_Click(object sender, System.Windows.RoutedEventArgs e) => ViewModel.RemoveCommand.SafeExecute();
    }
}
