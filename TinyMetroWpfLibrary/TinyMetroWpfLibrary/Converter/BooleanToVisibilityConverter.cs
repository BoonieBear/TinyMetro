// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TinyMetroWpfLibrary.Converter
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = false;
            if (parameter != null)
                bool.TryParse(parameter.ToString(), out invert);

            if (value == null)
                return Visibility.Collapsed;

            var isVisible = (bool)value;
            return isVisible ^ invert ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = false;
            if (parameter != null)
                bool.TryParse(parameter.ToString(), out invert);

            var visiblity = (Visibility)value;
            return (visiblity == Visibility.Visible) ^ invert;
        }
    }
}
