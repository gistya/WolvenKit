using System;
using System.Windows;

using WolvenKit.App.ViewModels.Events;
using WolvenKit.App.ViewModels.Shell;

namespace WolvenKit.Views.Editors
{
    /// <summary>
    /// Interaction logic for RedTypeView.xaml
    /// </summary>
    public partial class RedTypeView : System.Windows.Controls.UserControl
    {
        public RedTypeView()
        {
            InitializeComponent();
        }

        public event EventHandler ValueChanged;

        private void RedCNameEditor_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is not ChunkViewModel vm || e is not ValueChangedEventArgs args)
            {
                ValueChanged?.Invoke(sender, e);
                return;
            }

            args.RedType = vm.ResolvedData.GetType();
            ValueChanged?.Invoke(sender, args);
        }
    }
}
