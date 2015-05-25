using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace TinyMetroWpfLibrary.Controls
{
    public class DashArrowLinePointScaleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(values[0] is Point))
            {
                return null;
            }
            Point point = (Point)values[0];
            double x = point.X;
            double Y = point.Y;
            double Scale = (double)values[1];
            x *= Scale;
            Y *= Scale;
            return new Point(x, Y);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
