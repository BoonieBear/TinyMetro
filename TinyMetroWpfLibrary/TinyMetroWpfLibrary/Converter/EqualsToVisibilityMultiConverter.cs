// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TinyMetroWpfLibrary.Converter
{
    internal class EqualsToVisibilityMultiConverter : IMultiValueConverter
    {
        // object[0] and object[1] should be of the same type. If they're
        // equal, Convert returns Visibility.Visible, otherwise Visibility.Hidden

        // Optional object[2] is a boolean. If true, Convert returns
        // Visibility.Visible regardless.

        // Optional string parameter if "true" will reverse sense of
        // Visibility return values.

        public object Convert(object[] value, Type typeTarget,
                              object param, CultureInfo culture)
        {
            bool equals = value[0] == value[1];

            if (value.Length == 3)
            {
                if (value[2] is bool)
                    equals |= (bool) value[2];
            }

            bool reverse = false;

            if (param is string && Boolean.TryParse(param as string, out reverse) && reverse)
                return equals ? Visibility.Hidden : Visibility.Visible;

            return equals ? Visibility.Visible : Visibility.Hidden;
        }
        public object[] ConvertBack(object value, Type[] typeTarget,
                                    object param, CultureInfo culture)
        {
            return null;
        }
    }
}
