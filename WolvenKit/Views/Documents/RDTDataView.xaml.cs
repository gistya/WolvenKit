using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WolvenKit.App;
using WolvenKit.App.ViewModels.Documents;
using WolvenKit.App.ViewModels.Events;
using WolvenKit.Views.Editors;

namespace WolvenKit.Views.Documents
{
    /// <summary>
    /// Tree view for RDTData
    /// Interaction logic for RDTDataView.xaml
    /// </summary>
    public partial class RDTDataView : System.Windows.Controls.UserControl
    {
        public RDTDataViewModel ViewModel
        {
            get => DataContext as RDTDataViewModel;
            set => DataContext = value;
        }

        public RDTDataView()
        {
            InitializeComponent();

            if (DataContext is null)
            {
                DataContext = WolvenKit.AppImpl.Services?.GetService<RDTDataViewModel>();
            }

            var globals = WolvenKit.AppImpl.Services?.GetService<IOptions<Globals>>();
            if (globals?.Value.ENABLE_NODE_EDITOR == true)
            {
                // LayoutNodes() call can be re-enabled here or on Loaded if needed.
            }
                //{
                //    LayoutNodes();
                //};

                /*if (Globals.ENABLE_NODE_EDITOR)
                {
                    ViewModel.LayoutNodes = () => Editor.LayoutNodes();
                }*/

                //Editor.ItemsUpdated += (object sender, RoutedEventArgs e) =>
                //{
                //    LayoutNodes();
                //};

                //ViewModel.SelectedChunk.IsExpanded = true;

                //       viewmodel => viewmodel.SelectedChunk.PropertyGridData,
                //       view => view.PropertyGrid.SelectedObject)

                //       viewmodel => viewmodel.SelectedChunk.PropertyGridItems,
                //       view => view.PropertyGrid.Items)

                //       viewmodel => viewmodel.SelectedChunk,
                //       view => view.CustomPG.DataContext)

                //    .Where(x => x != null)
                //    .Select(x => new ObservableCollection<PropertyGridItem>(x.Properties
                //        .Where(x => x != null)
                //        .Select(x => new PropertyGridItem()
                //        {
                //            PropertyName = x.Name,
                //            Editor = PropertyGridEditors.GetPropertyEditor(x.PropertyType)
                //        }
                //    ))).BindTo(PropertyGrid, x => x.Items);


            // (Remaining commented reactive binding code removed in MVVM migration. XAML bindings or direct DC usage handle the rest.)

            //PropertyGrid.CustomEditorCollection = CustomEditorCollection;
            //MainTreeGrid.RequestTreeItems += TreeGrid_RequestTreeItems;
        }

        //public ICommand AddItemToArrayCommand { get; private set; }
        //public ICommand ExportChunkCommand { get; private set; }

        //private void HandleTemplateView_OnGoToChunkRequested(object sender, GoToChunkRequestedEventArgs e)
        //{
        //    var target = e.Export;

        //    if (ViewModel == null || target == null)
        //    {
        //        return;
        //    }

        //    var chunk = ViewModel.Chunks.FirstOrDefault(x => x.Name.Equals(target.REDName));
        //    chunk.IsSelected = true;
        //    ViewModel.SelectedChunk = chunk;
        //}


        private void AutolayoutNodes_MenuItem(object sender, RoutedEventArgs e) => Editor.LayoutNodes();

        private void RedTypeView_OnValueChanged(object sender, EventArgs e)
        {
            if (sender is not RedCNameEditor || e is not ValueChangedEventArgs args)
            {
                return;
            }

            ViewModel?.OnCNameValueChanged(args);
        }
    }
}
