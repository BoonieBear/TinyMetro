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
    /// Interaction logic for DatePickerFullMode.xaml
    /// </summary>
    public partial class DatePickerFullMode : Page
    {
        /// <summary>
        /// Initializes a new instance of the DatePickerFullMode class
        /// </summary>
        public DatePickerFullMode()
        {
            DataContext = new DatePickerFullModeViewModel();
            ((DatePickerFullModeViewModel)DataContext).Initialize();

            InitializeComponent();
            CorrectSelectorOrder();
        }

        /// <summary>
        /// Creates the selectors in the correct order
        /// </summary>
        private void CorrectSelectorOrder ()
        {
// ReSharper disable PossibleNullReferenceException
            var shortDatePattern = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            var separator = DateTimeFormatInfo.CurrentInfo.DateSeparator;
// ReSharper restore PossibleNullReferenceException

            // Ok. Split the date pattern and return the three parts in the correct order (e.g. dmy)
            shortDatePattern = shortDatePattern.ToUpperInvariant();
            var parts = shortDatePattern.Split(new []{separator}, StringSplitOptions.RemoveEmptyEntries)
                .Select((value, partIndex) => new { index = partIndex, part = value.First() })
                .ToDictionary(x=>x.part, x=>x.index);

            int index;
            if (parts.TryGetValue('D', out index))
                Grid.SetColumn(dayControl, index+1);

            if (parts.TryGetValue('M', out index))
                Grid.SetColumn(monthControl, index+1);

            if (parts.TryGetValue('Y', out index))
                Grid.SetColumn(yearControl, index+1);
        }

        /// <summary>
        ///  Change the is Active Part
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnWindowedItemsControlIsActiveChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (!((bool) args.NewValue)) 
                return;

            if (monthControl.IsManipulationInProgress && sender != monthControl)
                monthControl.FinishAnimation();

            if (dayControl.IsManipulationInProgress && sender != dayControl)
                dayControl.FinishAnimation();

            if (yearControl.IsManipulationInProgress && sender != yearControl)
                yearControl.FinishAnimation();

            monthControl.IsActive = sender == monthControl;
            dayControl.IsActive = sender == dayControl;
            yearControl.IsActive = sender == yearControl;
        }
    }
}
