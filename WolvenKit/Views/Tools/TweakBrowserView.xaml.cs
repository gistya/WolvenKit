using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WolvenKit.App.ViewModels.Tools;

namespace WolvenKit.Views.Tools
{
    /// <summary>
    /// Interaction logic for TweakBrowserView.xaml
    /// </summary>
    public partial class TweakBrowserView : System.Windows.Controls.UserControl
    {
        public TweakBrowserViewModel Context => (TweakBrowserViewModel)DataContext;

        public TweakBrowserView()
        {
            InitializeComponent();
            Loaded += (_, _) => Context.LoadTweakDB();
        }

        private void TextBox_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var tBox = (TextBox)sender;
                var prop = TextBox.TextProperty;

                BindingOperations.GetBindingExpression(tBox, prop)?.UpdateSource();
            }
        }
    }
}
