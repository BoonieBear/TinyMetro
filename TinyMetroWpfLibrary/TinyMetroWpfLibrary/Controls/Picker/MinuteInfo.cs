// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace BoonieBear.TinyMetro.WPF.Controls.Picker
{
    public class MinuteInfo
    {
        /// <summary>
        /// Initializes a new instance of the HourInfo class
        /// </summary>
        /// <param name="minute"></param>
        public MinuteInfo(int minute)
        {
            Minute = minute;
        }

        // 1 through 28, 29, 30, 31
        public int Minute { set; get; }
    }
}
