// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace BoonieBear.TinyMetro.WPF.Docking
{
    /// <summary>
    /// Provides methods for obtaining and modifying information for the system taskbar.
    /// Note:   This started out as this: http://www.crowsprogramming.com/archives/88       
    ///         I have modified it considerably to make the source read like the rest of my stuff
    ///         but Crows has the credit for building this wrapper.
    /// </summary>
    public static class TaskBar
    {
        #region Public Static Methods

        public static IntPtr GetHandle()
        {
            AppBarData appBar = GetTaskBarData();

            return appBar.Handle;
        }

        public static Point GetTaskBarLocation()
        {
            AppBarData appBar = GetTaskBarData();

            return new Point(appBar.Rectangle.left, appBar.Rectangle.top);
        }

        public static Size GetTaskBarSize()
        {
            AppBarData appBar = GetTaskBarData();

            return new Size(appBar.Rectangle.right - appBar.Rectangle.left, appBar.Rectangle.bottom - appBar.Rectangle.top);
        }

        public static TaskBarEdge GetTaskBarEdge()
        {
            AppBarData appBar = GetTaskBarData();

            return (TaskBarEdge)appBar.Edge;
        }

        public static TaskBarState GetTaskBarState()
        {
            AppBarData appBar = CreateAppBarData();

            return (TaskBarState)SHAppBarMessage(AbMsg.GetState, ref appBar);
        }

        public static void SetTaskBarState(TaskBarState state)
        {
            AppBarData appBar = CreateAppBarData();

            appBar.Parameter = (IntPtr)state;

            SHAppBarMessage(AbMsg.SetState, ref appBar);
        }

        #endregion

        #region Public Enums

        public enum TaskBarEdge
        {
            NotDocked = -1,
            Left = AbEdge.Left,
            Top = AbEdge.Top,
            Right = AbEdge.Right,
            Bottom = AbEdge.Bottom
        }

        [Flags]
        public enum TaskBarState
        {
            None = AbState.Manual,
            AutoHide = AbState.AutoHide,
            AlwaysTop = AbState.AlwaysOnTop
        }

        #endregion

        #region Private Static Methods

        private static AppBarData GetTaskBarData()
        {
            var appBar = CreateAppBarData();

            if (SHAppBarMessage(AbMsg.GetTaskBarPos, ref appBar) == (IntPtr)1)
            {
                return appBar;
            }
            throw new Exception("Unable to retrieve app bar information.");
        }

        private static AppBarData CreateAppBarData()
        {
            AppBarData appBar = new AppBarData();

            appBar.Handle = FindWindow("Shell_TrayWnd", "");
            appBar.Size = Marshal.SizeOf(appBar);

            return appBar;
        }


        #endregion

        #region P/Invoke

        #region Function Imports

        [DllImport("shell32.dll")]
        private static extern IntPtr SHAppBarMessage(AbMsg dwMessage, [In, Out] ref AppBarData pData);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        #endregion

        #region Private Enums

        private enum AbEdge
        {
            Left = 0,
            Top = 1,
            Right = 2,
            Bottom = 3,
        }

        private enum AbMsg
        {
            New = 0,
            Remove = 1,
            Querypos = 2,
            SetPos = 3,
            GetState = 4,
            GetTaskBarPos = 5,
            Activate = 6,
            GetAutoHideBar = 7,
            SetAutoHideBar = 8,
            WindowPosChanged = 9,
            SetState = 10
        }

        private enum AbState
        {
            Manual = 0,
            AutoHide = 1,
            AlwaysOnTop = 2,
            AutoHideAndOnTop = 3
        }

        #endregion

        #region Private Structs

        [StructLayout(LayoutKind.Sequential)]
        private struct AppBarData
        {
            public Int32 Size;
            public IntPtr Handle;
            public readonly Int32 CallbackMessage;
            public readonly AbEdge Edge;
            public RECT Rectangle;
            public IntPtr Parameter;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public readonly int left;
            public readonly int top;
            public readonly int right;
            public readonly int bottom;
        }

        #endregion

        #endregion
    }
}