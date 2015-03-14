// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace TinyMetroWpfLibrary.FrameControls.Picker
{
    public class YearInfo
    {
        public int YearNumber { set; get; }

        public override string ToString()
        {
            return YearNumber.ToString();
        }
    }
}
