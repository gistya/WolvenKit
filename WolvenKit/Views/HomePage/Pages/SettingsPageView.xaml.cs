using System.Windows.Controls;
using Syncfusion.Windows.PropertyGrid;
using WolvenKit.App.Services;
using WolvenKit.Controls;
using WolvenKit.ViewModels;
using static WolvenKit.Converters.PropertyGridEditors;
using Microsoft.Extensions.DependencyInjection;

namespace WolvenKit.Views.HomePage.Pages
{
    public partial class SettingsPageView : System.Windows.Controls.UserControl
    {
        public SettingsPageViewModel ViewModel
        {
            get => DataContext as SettingsPageViewModel;
            set => DataContext = value;
        }

        public SettingsPageView()
        {
            InitializeComponent();

            ViewModel = WolvenKit.AppImpl.Services?.GetService<SettingsPageViewModel>();
            DataContext = ViewModel;

        }

        public ItemCollection AccordionItems { get; set; }

        private void SettingsPropertygrid_OnAutoGeneratingPropertyGridItem(object sender, AutoGeneratingPropertyGridItemEventArgs e)
        {
// Generate special editors for the properties for which default is not ok
            if (e.OriginalSource is not PropertyItem { } propertyItem)
            {
                return;
            }

            switch (propertyItem.DisplayName)
            {
                case nameof(ISettingsDto.CP77ExecutablePath):
                    propertyItem.Editor =
                        new SingleFilePathEditor() { Filters = new PathEditorFilter[] { new("Cyberpunk2077.exe", "*.exe") } };
                    break;
                case nameof(ISettingsManager.MaterialRepositoryPath):
                    propertyItem.Editor = new SingleFolderPathEditor();
                    break;
                case nameof(ISettingsManager.ExtraModDirPath):
                    propertyItem.Editor = new SingleFolderPathEditor();
                    break;
                case nameof(ISettingsManager.DefaultEditorDifficultyLevel):
                    propertyItem.Editor = GetPropertyEditor(propertyItem.GetType());
                    break;
                case nameof(ISettingsDto.ThemeAccentString):
                    propertyItem.Editor = new BrushEditor();
                    break;
                default:
                    break;
            }
        }
    }
}
