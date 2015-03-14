using System;
using System.Runtime.InteropServices;

namespace TinyMetroWpfLibrary.Utility
{
    public class Win32Helper
    {
        public const int DIGCF_PRESENT = 2;
        public static Guid GUID_DEVCLASS_KEYBOARD = new Guid("4D36E96B-E325-11CE-BFC1-08002BE10318");
        
        [DllImport("setupapi.dll")]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid,
        IntPtr Enumerator, IntPtr hWndParent, int Flags);

        [DllImport("setupapi.dll")]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet,
        int Supplies, ref SP_DEVINFO_DATA DeviceInfoData);

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            public int cbSize;
            public Guid ClassGuid;
            public int DevInst;
            public int Reserved;
        }
        public const int CR_SUCCESS = 0;
        [DllImport("cfgmgr32.dll")]
        public static extern int CM_Get_Device_ID(int DevInst, IntPtr Buffer, int BufferLen, int Flags);

        public static string CM_Get_Device_ID(int DevInst)
        {
            string s = null;
            int len = 300;
            IntPtr buffer = Marshal.AllocHGlobal(len);
            int r = CM_Get_Device_ID(DevInst, buffer, len, 0);
            if (r == CR_SUCCESS) s = Marshal.PtrToStringAnsi(buffer);
            return s;
        }

    }
}
