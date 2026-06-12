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
