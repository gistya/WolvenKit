using System;
using System.Linq;
using NAudioWpfDemo.AudioPlaybackDemo;
using WolvenKit.App.ViewModels.Tools;

namespace WolvenKit.Views.Tools;
/// <summary>
/// Interaktionslogik für AudioPlayerView.xaml
/// </summary>
public partial class AudioPlayerView : System.Windows.Controls.UserControl
{
    public AudioPlayerView()
    {
        ViewModel = WolvenKit.AppImpl.Services?.GetService<AudioPlayerViewModel>();

        // TODO do this properly
        ViewModel.Visualizations.Add(new SpectrumAnalyzerVisualization());
        ViewModel.Visualizations.Add(new PolygonWaveFormVisualization());
        ViewModel.Visualizations.Add(new PolylineWaveFormVisualization());
        ViewModel.SelectedVisualization = ViewModel.Visualizations.FirstOrDefault();

        DataContext = ViewModel;

        InitializeComponent();
    }


}
