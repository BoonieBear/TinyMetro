using System;
using System.Globalization;
using System.Windows.Data;

namespace TinyMetroWpfLibrary.Controls.Converters
{
    public class ConverterToRevertValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double d = (double)value;
            return -d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
