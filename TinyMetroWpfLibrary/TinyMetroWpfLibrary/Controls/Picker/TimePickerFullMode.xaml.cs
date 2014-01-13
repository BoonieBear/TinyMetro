// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BoonieBear.TinyMetro.WPF.Controls.Picker
{
    /// <summary>
    /// Interaction logic for TimePickerFullMode.xaml
    /// </summary>
    public partial class TimePickerFullMode : Page
    {
        public TimePickerFullMode()
        {
            DataContext = new TimePickerFullModeViewModel();
            ((TimePickerFullModeViewModel)DataContext).Initialize();

            InitializeComponent();
            CorrectSelectorOrder();
        }


        /// <summary>
        /// Creates the selectors in the correct order
        /// </summary>
        private void CorrectSelectorOrder()
        {
            // ReSharper disable PossibleNullReferenceException
            var shortTimePattern = DateTimeFormatInfo.CurrentInfo.ShortTimePattern;
            var separator = DateTimeFormatInfo.CurrentInfo.TimeSeparator;
            // ReSharper restore PossibleNullReferenceException

            // Ok. Split the date pattern and return the three parts in the correct order (e.g. dmy)
            shortTimePattern = shortTimePattern.ToLowerInvariant();
            var parts = shortTimePattern.Split(new[] { separator, " " }, StringSplitOptions.RemoveEmptyEntries)
                .Select((value, partIndex) => new { index = partIndex, part = value.First() })
                .ToDictionary(x => x.part, x => x.index);

            int index;
            if (parts.TryGetValue('h', out index))
                Grid.SetColumn(hourControl, index + 1);

            if (parts.TryGetValue('m', out index))
                Grid.SetColumn(minuteControl, index + 1);

            if (parts.TryGetValue('t', out index))
                Grid.SetColumn(designatorControl, index + 1);
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

            if (designatorControl.IsManipulationInProgress && sender != designatorControl)
                designatorControl.FinishAnimation();

            hourControl.IsActive = sender == hourControl;
            minuteControl.IsActive = sender == minuteControl;
            designatorControl.IsActive = sender == designatorControl;
        }
    }
}
