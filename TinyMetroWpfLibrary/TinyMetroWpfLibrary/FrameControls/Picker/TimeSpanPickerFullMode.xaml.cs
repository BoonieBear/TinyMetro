// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;

namespace TinyMetroWpfLibrary.FrameControls.Picker
{
    /// <summary>
    /// Interaction logic for TimeSpanPickerFullMode.xaml
    /// </summary>
    public partial class TimeSpanPickerFullMode : Page
    {
        public TimeSpanPickerFullMode()
        {
            DataContext = new TimeSpanPickerFullModeViewModel();
            ((TimeSpanPickerFullModeViewModel)DataContext).Initialize();

            InitializeComponent();
            CorrectSelectorOrder();
        }

        /// <summary>
        /// Creates the selectors in the correct order
        /// </summary>
        private void CorrectSelectorOrder()
        {
            Grid.SetColumn(hourControl, 1);
            Grid.SetColumn(minuteControl, 2);
        }

        /// <summary>
        ///  Change the is Active Part
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnWindowedItemsControlIsActiveChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (!((bool)args.NewValue))
                return;

            if (hourControl.IsManipulationInProgress && sender != hourControl)
                hourControl.FinishAnimation();

            if (minuteControl.IsManipulationInProgress && sender != minuteControl)
                minuteControl.FinishAnimation();

            hourControl.IsActive = sender == hourControl;
            minuteControl.IsActive = sender == minuteControl;
        }
    }
}
