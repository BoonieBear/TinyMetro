// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows.Data;

namespace TinyMetroWpfLibrary.Converter
{
    /// <summary>
    /// This Converter is used to show a date value as a short time string
    /// </summary>
    public class ShortTimeConverter : IValueConverter
    {
        private DateTime fallbackTime;

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeValue = (DateTime)value;
            fallbackTime = timeValue;
            return timeValue.ToShortTimeString();
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            culture = System.Threading.Thread.CurrentThread.CurrentUICulture;

            // Try to convert to double
            double dblValue;
            if (double.TryParse(value.ToString(), NumberStyles.Number, culture.NumberFormat, out dblValue))
                return fallbackTime.Date.AddHours(Math.Max(0, Math.Min(23.99, dblValue)));

            // Try to convert with DateTime first
            DateTime result;
            if (DateTime.TryParse(value.ToString(), culture.DateTimeFormat, DateTimeStyles.None, out result))
                return result;

            return fallbackTime;
        }
    }
}
