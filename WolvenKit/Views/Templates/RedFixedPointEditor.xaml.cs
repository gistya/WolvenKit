using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WolvenKit.App.ViewModels.Shell;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Editors
{
    /// <summary>
    /// Interaction logic for RedFixedPointEditor.xaml
    /// </summary>
    public partial class RedFixedPointEditor : UserControl
    {
        public ChunkViewModel cvm => DataContext as ChunkViewModel;

        public RedFixedPointEditor()
        {
            InitializeComponent();

                        var throttleTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(.5)
            };
            throttleTimer.Tick += (_, _) =>
            {
                throttleTimer.Stop();
                SetRedValue(TextBox.Text);
            };
            TextBox.TextChanged += (_, _) =>
            {
                throttleTimer.Stop();
                throttleTimer.Start();
            };
        }

        public FixedPoint RedNumber
        {
            get => (FixedPoint)GetValue(RedNumberProperty);
            set => SetValue(RedNumberProperty, value);
        }
        public static readonly DependencyProperty RedNumberProperty = DependencyProperty.Register(
            nameof(RedNumber), typeof(FixedPoint), typeof(RedFixedPointEditor), new PropertyMetadata(default(FixedPoint)));


        public string Text
        {
            get => GetValueFromRedValue();
            set => SetRedValue(value);
        }

        private void SetRedValue(string value)
        {
            try
            {
                if (cvm != null)
                {
                    cvm.Data = (FixedPoint)float.Parse(value);
                }
                else
                {
                    SetCurrentValue(RedNumberProperty, (FixedPoint)float.Parse(value));
                }
            }
            catch (FormatException)
            {

            }
        }

        private string GetValueFromRedValue() => cvm != null ? ((float)(FixedPoint)cvm.Data).ToString("G9") : ((float)RedNumber).ToString("G9");
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var tb = (TextBox)e.Source;
            e.Handled = !float.TryParse(tb.Text.Insert(tb.CaretIndex, e.Text), out _);
        }
    }
}
