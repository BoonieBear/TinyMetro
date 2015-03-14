using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TinyMetroWpfLibrary.Helper
{
    /// <summary>
    /// This class is used to get the dpi of the screen
    /// </summary>
    public class ScreenResolution
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        /// <summary>
        /// Initializes a new instance of the ScreenResolution class
        /// </summary>
        public ScreenResolution()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();

            Xdpi = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSX);
            Ydpi = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSY);  
        }

        private enum DeviceCap
        {
            /// <summary> 
            /// Logical pixels inch in X 
            /// </summary> 
            LOGPIXELSX = 88,
            /// <summary> 
            /// Logical pixels inch in Y 
            /// </summary> 
            LOGPIXELSY = 90

            // Other constants may be founded on pinvoke.net 
        }

        /// <summary>
        /// Gets or sets the X Dpi
        /// </summary>
        public int Xdpi { get; set; }

        /// <summary>
        /// gets or sets the Y Dpi
        /// </summary>
        public int Ydpi { get; set; }

        /// <summary>
        /// Converts the given integer to the screen dpi
        /// </summary>
        public double ConvertXDpi(double x)
        {
            return x * 96.0 / Xdpi;
        }

        /// <summary>
        /// Converts the given integer to the screen dpi
        /// </summary>
        public double ConvertYDpi(double y)
        {
            return y * 96.0 / Ydpi;
        }

        /// <summary>
        /// Converts the given form paramter to screen coordinates
        /// </summary>
        public double ConvertXToScreen(double x)
        {
            return x * Xdpi / 96.0;
        }

        /// <summary>
        /// Converts the given form paramter to screen coordinates
        /// </summary>
        public double ConvertYToScreen(double y)
        {
            return y * Ydpi / 96.0;
        }
    }
}
