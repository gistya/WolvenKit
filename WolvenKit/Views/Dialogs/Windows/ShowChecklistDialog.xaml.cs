using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WolvenKit.App.Interaction.Options;
using WolvenKit.App.Services;
using WolvenKit.App.ViewModels.Dialogs;
using WolvenKit.Converters;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Window = System.Windows.Window;

namespace WolvenKit.Views.Dialogs.Windows;


public partial class ShowChecklistDialog
{
    private static List<string> s_lastSelection = [];
    private static string s_lastInputFieldText = "";


    public ShowChecklistDialog(ChecklistDialogOptions options)
    {
        InitializeComponent();

        if (s_lastInputFieldText != "" && options.InputFieldLabel != "")
        {
            options.InputFieldDefaultValue = s_lastInputFieldText;
        }

        foreach (var se in s_lastSelection.Where(options.ChecklistOptions.ContainsKey))
        {
            options.ChecklistOptions[se] = true;
        }

        ViewModel = new ShowChecklistDialogViewModel(options);
        DataContext = ViewModel;

        {
            // bind to filteredChecklistMenu
                    x => x.ChecklistOptions,
                    x => x.FilterableChecklistMenu.CheckboxOptionsAndStates)

            // bind to selectedOptions
            // Note: converter overrides removed with ReactiveUI migration. Collection conversion handled inside the menu or VM if needed.
                    x => x.SelectedOptions,
                    x => x.FilterableChecklistMenu.SelectedOptions)

            // set filter text (if given)
            FilterableChecklistMenu.SetCurrentValue(Templates.FilterableChecklistMenu.FilterTextProperty,
                options.FilterDefaultValue);

            // Load last selection if available
            ViewModel.SelectedOptions = options.ChecklistOptions.Where(x => x.Value)
                .Select(x => x.Key).ToList();

            // bind rest of properties
                    x => x.InputFieldText,
                    x => x.TextInputBox.Text)

                    x => x.RememberValues,
                    x => x.RememberValuesCheckBox.IsChecked)
        });
    }


    public ShowChecklistDialogViewModel ViewModel { get; set; }

    {
        get => ViewModel;
        set => ViewModel = (ShowChecklistDialogViewModel)value;
    }

    public bool? ShowDialog(Window owner)
    {
        Owner = owner;
        return ShowDialog();
    }


    private void SaveLastSelection()
    {
        if (ViewModel?.RememberValues != true)
        {
            s_lastSelection.Clear();
            s_lastInputFieldText = "";
            return;
        }

        s_lastSelection = ViewModel.SelectedOptions.ToList();
        s_lastInputFieldText = ViewModel.InputFieldText;
    }

    private void WizardPage_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.A && ModifierViewStateService.IsCtrlBeingHeld && ViewModel is not null)
        {
            FilterableChecklistMenu.ToggleSelection();
            return;
        }
        if (e.Key != Key.Enter)
        {
            return;
        }

        SaveLastSelection();
        e.Handled = true;
        DialogResult = true;
        Close();
    }

    private void WizardControl_OnFinish(object sender, RoutedEventArgs e) => SaveLastSelection();

    private void ChecklistMenu_OnSelectionChanged(object _, List<string> selection)
    {
        if (ViewModel is null)
        {
            return;
        }

        ViewModel.SelectedOptions ??= [];

        ViewModel.SelectedOptions.Clear();
        ViewModel.SelectedOptions.AddRange(selection);
    }
}
