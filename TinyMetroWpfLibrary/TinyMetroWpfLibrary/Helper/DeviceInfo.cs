using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace TinyMetroWpfLibrary.Helper
{
    /// <summary>
    /// This class offers methods for getting device infos
    /// </summary>
    public static class DeviceInfo
    {
        /// <summary>
        /// Return True, if the device has a touch input
        /// </summary>
        /// <returns></returns>
        public static bool HasTouchInput()
        {
            return Tablet.TabletDevices.Cast<TabletDevice>().Any(tabletDevice => tabletDevice.Type == TabletDeviceType.Touch);
        }
    }
}
