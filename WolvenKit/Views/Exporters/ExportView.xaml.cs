using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.PropertyGrid;
using WolvenKit.App.ViewModels.Exporters;
using WolvenKit.App.ViewModels.Tools;
using WolvenKit.Common.Model.Arguments;
using WolvenKit.Helpers;

namespace WolvenKit.Views.Exporters;

/// <summary>
/// Interaction logic for ExportView.xaml
/// </summary>
public partial class ExportView : System.Windows.Controls.UserControl
{
    private readonly Dictionary<string, PropertyInfo> _shownProperties = new();

    public ExportView()
    {
        InitializeComponent();

        ExportGrid.FilterRowCellRenderers.Add("TextBoxExt", new GridFilterRowTextBoxRendererExt());
        ExportGrid.FilterChanged += Datagrid_FilterChanged;

        if (DataContext is null)
        {
            DataContext = WolvenKit.AppImpl.Services?.GetService<ExportViewModel>();
        }

        if (DataContext is ExportViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.OnRefresh += RefreshFilter;

            // Reactive binds replaced with direct assignment + XAML where possible
            ExportGrid.ItemsSource = viewModel.Items;
            // For two-way selected and property grid, we rely on XAML bindings or further sync if needed
        }
    }

    public ExportViewModel ViewModel { get; set; }


    private void RefreshFilter(object sender, EventArgs e) => Datagrid_FilterChanged(ExportGrid, null);
    private void Datagrid_FilterChanged(object sender, GridFilterEventArgs e)
    {
        if (sender is not SfDataGrid grid || ViewModel is null)
        {
            return;
        }

        ViewModel.VisibleItemPaths = grid.View.Records
            .Select(record => record.Data).OfType<ImportExportItemViewModel>()
            .Select(m => m.BaseFile)
            .ToList();
    }

    private object _previousSelectedObject;

    private void OverlayPropertyGrid_AutoGeneratingPropertyGridItem(object sender, AutoGeneratingPropertyGridItemEventArgs e)
    {
        if (!ReferenceEquals(_previousSelectedObject, OverlayPropertyGrid.SelectedObject))
        {
            _shownProperties.Clear();
            _previousSelectedObject = OverlayPropertyGrid.SelectedObject;
        }
// Generate special editors for certain properties
        // we need the callback function
        // we need the propertyname
        // we need the type of the arguments
        if (ViewModel is null ||
            e.OriginalSource is not PropertyItem propertyItem ||
            sender is not PropertyGrid { SelectedObject: ExportArgs args })
        {
            return;
        }

        if (Attribute.GetCustomAttribute(propertyItem.PropertyInformation, typeof(UsedWith)) is UsedWith usedWith)
        {
            if (!_shownProperties.TryGetValue(usedWith.PropertyName, out var propertyInfo))
            {
                e.Cancel = true;
                return;
            }

            var value = propertyInfo.GetValue(args);

            if (!usedWith.Values.Contains(value))
            {
                e.Cancel = true;
                return;
            }
        }

        switch (propertyItem.DisplayName)
        {
            case nameof(MeshExportArgs.Rig):
            case nameof(MeshExportArgs.MultiMeshMeshes):
            case nameof(MeshExportArgs.MultiMeshRigs):
            case nameof(OpusExportArgs.SelectedForExport):
                propertyItem.Editor = new CustomCollectionEditor(ViewModel.InitCollectionEditor,
                    new CallbackArguments(args, propertyItem.DisplayName));
                break;
            default:
                break;
        }

        _shownProperties.Add(propertyItem.Name, propertyItem.PropertyInformation);
    }

    private void OverlayPropertyGrid_OnValueChanged(object sender, ValueChangedEventArgs args)
    {
        _shownProperties.Clear();
        OverlayPropertyGrid.RefreshPropertygrid();
    }

    private void ExportGrid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
    {
        foreach (var item in e.AddedItems)
        {
            if (item is GridRowInfo { RowData: ImportExportItemViewModel vm })
            {
                vm.IsChecked = true;
            }
        }

        foreach (var item in e.RemovedItems)
        {
            if (item is GridRowInfo { RowData: ImportExportItemViewModel vm } &&
                !e.AddedItems.Contains(item))
            {
                vm.IsChecked = false;
            }
        }

        ViewModel?.ProcessSelectedCommand.NotifyCanExecuteChanged();
        ViewModel?.CopyArgumentsTemplateToCommand.NotifyCanExecuteChanged();
        ViewModel?.PasteArgumentsTemplateToCommand.NotifyCanExecuteChanged();
        ViewModel?.ImportSettingsCommand.NotifyCanExecuteChanged();
    }
}

