// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;

namespace BoonieBear.TinyMetro.WPF.Controls.Picker
{
    public class DayInfo
    {
        // 1 through 28, 29, 30, 31
        public int DayNumber { set; get; }

        public int Year { set; get; }
        public int Month { set; get; }

        public string DayOfWeek
        {
            get
            {
                return DateTimeFormatInfo.CurrentInfo.DayNames[(int)new DateTime(Year, Month, DayNumber).DayOfWeek];
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", DayNumber, DayOfWeek);
        }
    }
}
