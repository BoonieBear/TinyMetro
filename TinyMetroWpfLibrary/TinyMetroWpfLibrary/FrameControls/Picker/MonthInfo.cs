// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;

namespace TinyMetroWpfLibrary.FrameControls.Picker
{
    public class MonthInfo
    {
        // 1 through 12
        public int MonthNumber { set; get; }

        public string MonthName
        {
            get
            {
                return DateTimeFormatInfo.CurrentInfo.MonthNames[MonthNumber - 1];
            }
        }

        public override string ToString()
        {
            return MonthName;
        }
    }
}
