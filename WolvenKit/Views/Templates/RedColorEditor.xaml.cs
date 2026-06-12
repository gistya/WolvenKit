using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using WolvenKit.App.ViewModels.Shell;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Editors
{
    /// <summary>
    /// Interaction logic for RedColorEditor.xaml
    /// </summary>
    public partial class RedColorEditor : UserControl
    {
        private ChunkViewModel _cvm => DataContext as ChunkViewModel;

        public RedColorEditor()
        {
            InitializeComponent();

            SelectedColorCommand = new RelayCommand<object>(OnColorPicked);
        }

        public CColor RedColor
        {
            get => (CColor)GetValue(RedColorProperty);
            set => SetValue(RedColorProperty, value);
        }
        public static readonly DependencyProperty RedColorProperty = DependencyProperty.Register(
            nameof(RedColor), typeof(CColor), typeof(RedColorEditor), new PropertyMetadata(default(CColor)));

        public ICommand SelectedColorCommand { get; }

        public Color Color
        {
            get => GetValueFromRedValue();
            set => SetRedValue(value);
        }

        private void OnColorPicked(object e)
        {
            dynamic d = e;
            var mediaColor = d.Brush.Color;
            RedColor.Alpha = mediaColor.A;
            RedColor.Red = mediaColor.R;
            RedColor.Green = mediaColor.G;
            RedColor.Blue = mediaColor.B;

            _cvm.NotifyChain(nameof(ChunkViewModel.Data));
            _cvm.RecalculateProperties();
        }

        private void SetRedValue(Color value)
        {
            dynamic d = value;
            var mediaColor = d.Brush.Color;

            RedColor.Alpha = mediaColor.A;
            RedColor.Red = mediaColor.R;
            RedColor.Green = mediaColor.G;
            RedColor.Blue = mediaColor.B;
        }

        private Color GetValueFromRedValue()
        {
            var redvalue = new Color()
            {
                A = RedColor.Alpha,
                R = RedColor.Red,
                G = RedColor.Green,
                B = RedColor.Blue
            };
            return redvalue;
        }


    }
}
