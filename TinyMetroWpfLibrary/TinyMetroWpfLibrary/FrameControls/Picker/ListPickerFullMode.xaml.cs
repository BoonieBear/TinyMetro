// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;
using System.Windows.Input;

namespace TinyMetroWpfLibrary.FrameControls.Picker
{
    /// <summary>
    /// Interaction logic for ListPickerFullMode.xaml
    /// </summary>
    public partial class ListPickerFullMode : Page
    {
        /// <summary>
        /// Initializes a new instance of the ListPicerFullMode class
        /// </summary>
        public ListPickerFullMode()
        {
            DataContext = new ListPickerFullModeViewModel();
            ((ListPickerFullModeViewModel)DataContext).Initialize();

            InitializeComponent();
            FilterTextBox.Focus();
        }

        /// <summary>
        /// Selected the current item
        /// </summary>
        /// <param name="sender">sending listpicker</param>
        /// <param name="e">event arguments</param>
        private void OnSelectItem(object sender, MouseButtonEventArgs e)
        {
            ExecuteItemSelection(sender);
        }

        /// <summary>
        /// Select an item either by mouse or by touch screen
        /// </summary>
        /// <param name="sender"></param>
        private void ExecuteItemSelection(object sender)
        {
            var box = sender as ListBox;
            if (box == null)
                return;

            if (box.SelectedItem == null && box.Items.Count == 1)
                box.SelectedItem = box.Items.GetItemAt(0);

            if (box.SelectedItem == null)
                return;

            var context = ((ListPickerFullModeViewModel)DataContext);
            if (context.CheckCommand.CanExecute(null))
                context.CheckCommand.Execute(null);
        }

        /// <summary>
        /// Listen to the key up event of the return in order to select the item
        /// </summary>
        /// <param name="sender">Listbox</param>
        /// <param name="e">key event</param>
        private void OnListBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ExecuteItemSelection(sender);
            }
        }
    }
}
