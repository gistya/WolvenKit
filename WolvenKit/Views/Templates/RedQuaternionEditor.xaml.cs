using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using WolvenKit.App.ViewModels.Shell;
using WolvenKit.RED4.Types;

namespace WolvenKit.Views.Editors
{
    /// <summary>
    /// Interaction logic for RedQuaternionEditor.xaml
    /// </summary>
    public partial class RedQuaternionEditor : UserControl
    {
        public ChunkViewModel cvm => DataContext as ChunkViewModel;

        public RedQuaternionEditor()
        {
            InitializeComponent();

            AttachThrottledTextChanged(ITextBox, SetIValue);
            AttachThrottledTextChanged(JTextBox, SetJValue);
            AttachThrottledTextChanged(KTextBox, SetKValue);
            AttachThrottledTextChanged(RTextBox, SetRValue);

        }

        private void AttachThrottledTextChanged(TextBox textBox, Action<string> setter)
        {
            var throttleTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(.5)
            };
            throttleTimer.Tick += (_, _) =>
            {
                throttleTimer.Stop();
                setter(textBox.Text);
            };
            textBox.TextChanged += (_, _) =>
            {
                throttleTimer.Stop();
                throttleTimer.Start();
            };
        }

        //public Quaternion RedQuaternion
        //{
        //    get => (Quaternion)this.GetValue(RedQuaternionProperty);
        //    set => this.SetValue(RedQuaternionProperty, value);
        //}
        //public static readonly DependencyProperty RedQuaternionProperty = DependencyProperty.Register(
        //    nameof(RedQuaternion), typeof(Quaternion), typeof(RedQuaternionEditor), new PropertyMetadata(default(Quaternion)));


        public string IText
        {
            get => GetValueFromIValue();
            set => SetIValue(value);
        }

        public string JText
        {
            get => GetValueFromJValue();
            set => SetJValue(value);
        }

        public string KText
        {
            get => GetValueFromKValue();
            set => SetKValue(value);
        }

        public string RText
        {
            get => GetValueFromRValue();
            set => SetRValue(value);
        }

        private void SetIValue(string value)
        {
            ((Quaternion)cvm.Data).I = float.Parse(value);
            var x = cvm.Properties?.FirstOrDefault(prop => prop.Name == "i") ?? null;
            if (x != null)
            {
                x.Data = (CFloat)float.Parse(value);
            }
        }

        private void SetJValue(string value)
        {
            ((Quaternion)cvm.Data).J = float.Parse(value);
            var x = cvm.Properties?.FirstOrDefault(prop => prop.Name == "j") ?? null;
            if (x != null)
            {
                x.Data = (CFloat)float.Parse(value);
            }
        }

        private void SetKValue(string value)
        {
            ((Quaternion)cvm.Data).K = float.Parse(value);
            var x = cvm.Properties?.FirstOrDefault(prop => prop.Name == "k") ?? null;
            if (x != null)
            {
                x.Data = (CFloat)float.Parse(value);
            }
        }

        private void SetRValue(string value)
        {
            ((Quaternion)cvm.Data).R = float.Parse(value);
            var x = cvm.Properties?.FirstOrDefault(prop => prop.Name == "r") ?? null;
            if (x != null)
            {
                x.Data = (CFloat)float.Parse(value);
            }
        }

        private string GetValueFromIValue() => ((float)((Quaternion)cvm.Data).I).ToString("G9");

        private string GetValueFromJValue() => ((float)((Quaternion)cvm.Data).J).ToString("G9");

        private string GetValueFromKValue() => ((float)((Quaternion)cvm.Data).K).ToString("G9");

        private string GetValueFromRValue() => ((float)((Quaternion)cvm.Data).R).ToString("G9");


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var tb = (TextBox)e.Source;
            e.Handled = !float.TryParse(tb.Text.Insert(tb.CaretIndex, e.Text), out _);
        }

    }
}
