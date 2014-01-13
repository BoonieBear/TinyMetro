// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;

namespace BoonieBear.TinyMetro.WPF.Controls.Picker
{
    public class DesignatorInfo
    {
        /// <summary>
        /// Initializes a new instance of the DesignatorInfo
        /// </summary>
        /// <param name="number"></param>
        public DesignatorInfo(int number)
        {
            DesignatorNumber = number;
        }

        // 0 through 1
        public int DesignatorNumber { set; get; }

        /// <summary>
        /// Returns the desginator as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DesignatorNumber == 0 ? DateTimeFormatInfo.CurrentInfo.AMDesignator : DateTimeFormatInfo.CurrentInfo.PMDesignator;
        }
    }
}
