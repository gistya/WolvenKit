using System.Windows;
using System.Windows.Controls;
using WolvenKit.App.ViewModels.Shell;
using WolvenKit.RED4.Types;
using WolvenKit.Views.Templates;

namespace WolvenKit.Views.Editors
{
    /// <summary>
    /// CName editor with optional dropdown
    /// </summary>
    public partial class FilterableDropdownCNameMenu : FilterableDropdownMenuBase<CName>
    {
        public FilterableDropdownCNameMenu()
        {
            InitializeComponent();
            _useDefaultOption = true;

            {
                if (DataContext is not ChunkViewModel vm)
                {
                    return;
                }

                        v => (CName)v.Data,
                        x => x.RedCNameEditor.RedString)

                InitializePropertyValues(vm);

                if (!ShowRefreshButton)
                {
                    ColumnRefreshButton.SetCurrentValue(ColumnDefinition.WidthProperty, new GridLength(0));
                }

                if (Options.Count != 0 || ShowRefreshButton)
                {
                    return;
                }

                // If we don't have any options, no reason to show the dropdown - disable these UI elements
                // and show only the default editor
                FilterRow.SetCurrentValue(RowDefinition.MinHeightProperty, 0.0);
                FilterRow.SetCurrentValue(RowDefinition.HeightProperty, new GridLength(0));
                FilterTextBox.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                Dropdown.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
            });
        }


        protected override void SetChunkViewModelValueFromDropdown()
        {
            if (string.IsNullOrWhiteSpace(SelectedOption)
                || DataContext is not ChunkViewModel { Data: CName } cvm)
            {
                return;
            }

            cvm.Data = (CName)SelectedOption;

        }

        private void RedCNameEditor_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ChunkViewModel { Data: CName var } && ((var.GetResolvedText() ?? "") != SelectedOption))
            {
                SetDropdownValueFromDataContext();
            }
        }

        protected override void ResetDropdownValue() => Dropdown.SetCurrentValue(ComboBox.TextProperty, "");
    }
}
