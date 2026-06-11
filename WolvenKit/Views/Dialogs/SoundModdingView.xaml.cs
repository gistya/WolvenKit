using System;
using System.Collections.Generic;
using System.Linq;
using WolvenKit.App.ViewModels.Dialogs;
using WolvenKit.Modkit.RED4.Sounds;
using WolvenKit.App.Extensions;

namespace WolvenKit.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for SoundModdingView.xaml
    /// </summary>
    public partial class SoundModdingView : System.Windows.Controls.UserControl
    {
        public SoundModdingView()
        {
            InitializeComponent();


            {
                    vm => vm.SoundEvents,
                    v => v.DataGridEvents.ItemsSource)


            });
        }

        private void ButtonAddAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectedtems = new List<SoundEvent>();
            foreach (var item in DataGridEvents.SelectedItems)
            {
                if (item is SoundEvent soundEvent)
                {
                    selectedtems.Add(soundEvent);
                }
            }

            ViewModel.SelectedEvents = selectedtems;
            ViewModel.AddCommand.SafeExecute();
        }

        private List<string> _selectedtems = new();
        private void ComboBoxAdv_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedtems = new List<string>();
            foreach (var item in ComboBoxTags.SelectedItems)
            {
                if (item is string tag)
                {
                    _selectedtems.Add(tag);
                }
            }

            DataGridEvents.View.Filter = FilterRecords;
            DataGridEvents.View.RefreshFilter();
        }

        public bool FilterRecords(object o)
        {
            if (o is SoundEvent item)
            {
                if (_selectedtems.Count == 0)
                {
                    return true;
                }
                if (item.Tags.Any(x => _selectedtems.Contains(x)))
                {
                    return true;
                }
            }
            return false;
        }

        private void ButtonAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            var item = DataGridEvents.SelectedItem as SoundEvent;
            var selectedtems = new List<SoundEvent>
            {
                item
            };
            ViewModel.SelectedEvents = selectedtems;
            ViewModel.AddCommand.SafeExecute();
        }

        private void ButtonDel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = PluginList.SelectedItem as CustomSoundsModel;
            ViewModel.CustomEvents.Remove(item);
        }
    }
}
