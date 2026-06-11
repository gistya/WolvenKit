using System;
using System.Windows;
using System.Windows.Controls;
using WolvenKit.App.ViewModels.Dialogs;

namespace WolvenKit.Views.Dialogs;
/// <summary>
/// Interaktionslogik für ProjectSettingsDialog.xaml
/// </summary>
public partial class ProjectSettingsDialog : System.Windows.Controls.UserControl
{
    public ProjectSettingsDialog()
    {
        InitializeComponent();

        {






        });

            .Subscribe(selectedItem =>
            {
                if (selectedItem is ListBoxItem { Content: string name })
                {
                    if (name == "General")
                    {
                        GeneralPanel.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                    }
                }
            });
    }
}
