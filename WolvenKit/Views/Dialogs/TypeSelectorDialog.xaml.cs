using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WolvenKit.App.ViewModels.Dialogs;
using WolvenKit.Helpers;

namespace WolvenKit.Views.Dialogs;
/// <summary>
/// Interaktionslogik für TypeSelectorDialog.xaml
/// </summary>
public partial class TypeSelectorDialog : System.Windows.Controls.UserControl
{
    public TypeSelectorDialog()
    {
        InitializeComponent();

        TypeDataGrid.FilterRowCellRenderers.Add("TextBoxExt", new GridFilterRowTextBoxRendererExt());

        {
            TypeDataGrid.ClearFilters();




        });
    }
}
