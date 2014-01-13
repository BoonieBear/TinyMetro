// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Linq;
using BoonieBear.TinyMetro.WPF.Helper;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace BoonieBear.TinyMetro.WPF.Docking
{
    /// <summary>
    /// Implements a application task bar docking mechanism
    /// 
    /// This Docker, originally implemented by ColbyAfrica.Components.TaskBarDocker
    /// has been extended in order to enable a window to dock to all edges.
    /// 
    /// For more info see <see cref="http://code.msdn.microsoft.com/windowsdesktop/Task-Bar-Docking-Component-0975173a/view/SourceCode"/>
    /// </summary>
    public partial class TaskBarDocker
    {
        #region Instance Data

        private readonly Window form;
        private readonly Timer timer;
        private bool isFirstRun = true;
        private TaskBar.TaskBarEdge lastEdge;
        private Size lastSize;
        private Point lastLocation;
        private TaskBar.TaskBarEdge dockedTo = TaskBar.TaskBarEdge.NotDocked;

        public event Action<TaskBar.TaskBarEdge> OnDockingChanged = null;

        /// <summary>
        /// Used to get the DPI of the screen
        /// </summary>
        private readonly ScreenResolution screenResolution = new ScreenResolution();
        
        #endregion

        #region Private Constants
        private const int TIMER_INTERVAL = 1000;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the TaskBarDocker class
        /// </summary>
        /// <param name="form">WCF Window that needs to be adjusted</param>
        /// <param name="attachToTaskBarEvent"></param>
        public TaskBarDocker(Window form, bool attachToTaskBarEvent)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            this.form = form;
            AttachedToTaskBarEvent = attachToTaskBarEvent;

            if (attachToTaskBarEvent)
            {
                timer = new Timer {Interval = TIMER_INTERVAL};
                timer.Tick += TimerTick;
                timer.Enabled = true;
            }
        }

        #endregion

        #region Public Properties

        public bool AttachedToTaskBarEvent { get; set; }

        public TaskBar.TaskBarEdge Position
        {
            get
            {
                return TaskBar.GetTaskBarEdge();
            }
        }

        #endregion

        #region Public Methods

        public void Dock(TaskBar.TaskBarEdge dockTo, int nr)
        {
            // Store the current docking
            dockedTo = dockTo;
            activeScreenIndex = nr;

            switch (dockTo)
            {
                case TaskBar.TaskBarEdge.Left:
                    PostionLeft();
                    break;
                case TaskBar.TaskBarEdge.Top:
                    PostionTop();
                    break;
                case TaskBar.TaskBarEdge.Right:
                    PostionRight();
                    break;
                case TaskBar.TaskBarEdge.Bottom:
                    PostionBottom();
                    break;

                case TaskBar.TaskBarEdge.NotDocked:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (OnDockingChanged != null)
                OnDockingChanged(dockTo);
        }

        #endregion

        #region Private Methods


        private void PostionLeft()
        {
            Screen activeScreen = ActiveScreen;
            Screen taskBarScreen = Screen.FromHandle(TaskBar.GetHandle()); ;
            Size taskBarSize = TaskBar.GetTaskBarSize();

            var left = activeScreen.Bounds.Left;
            var top = activeScreen.Bounds.Top;
            var height = activeScreen.Bounds.Height;

            var taskbarHeight = (activeScreen.DeviceEquals(taskBarScreen) ? taskBarSize.Height : 0);
            var taskbarWidth = (activeScreen.DeviceEquals(taskBarScreen) ? taskBarSize.Width : 0); 

            switch (TaskBar.GetTaskBarEdge())
            {
                case TaskBar.TaskBarEdge.Left:
                    form.Left = screenResolution.ConvertXDpi(left + taskbarWidth);
                    form.Top = screenResolution.ConvertYDpi(top);
                    form.Height = screenResolution.ConvertYDpi(height);
                    break;

                case TaskBar.TaskBarEdge.Top:
                    form.Left = screenResolution.ConvertXDpi(left);
                    form.Top = screenResolution.ConvertYDpi(top + taskbarHeight);
                    form.Height = screenResolution.ConvertYDpi(height - taskbarHeight);
                    break;

                case TaskBar.TaskBarEdge.Right:
                    form.Left = screenResolution.ConvertXDpi(left);
                    form.Top = screenResolution.ConvertYDpi(top);
                    form.Height = screenResolution.ConvertYDpi(height);
                    break;

                case TaskBar.TaskBarEdge.Bottom:
                    form.Left = screenResolution.ConvertXDpi(left);
                    form.Top = screenResolution.ConvertYDpi(top);
                    form.Height = screenResolution.ConvertYDpi(height - taskbarHeight);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public TaskBar.TaskBarEdge DockedTo
        {
            get { return dockedTo; }
        }

        public Screen ActiveScreen
        {
            get { return Screen.AllScreens[ActiveScreenIndex]; }
        }

        private int activeScreenIndex = 0;
        public int ActiveScreenIndex 
        {
            get { return activeScreenIndex; }
            set { activeScreenIndex = Math.Max(0, Math.Min(value, Screen.AllScreens.Count()-1)); }
        }

        private void PostionTop()
        {
            Screen activeScreen = ActiveScreen;
            Screen taskBarScreen = Screen.FromHandle(TaskBar.GetHandle()); ;
            Size taskBarSize = TaskBar.GetTaskBarSize();

            var left = activeScreen.Bounds.Left;
            var top = activeScreen.Bounds.Top;
            var width = activeScreen.Bounds.Width;

            var taskbarHeight = (activeScreen.DeviceEquals(taskBarScreen) ? taskBarSize.Height : 0);
            var taskbarWidth = (activeScreen.DeviceEquals(taskBarScreen) ? taskBarSize.Width : 0); 

            switch (TaskBar.GetTaskBarEdge())
            {
                case TaskBar.TaskBarEdge.Left:
                    form.Top = screenResolution.ConvertYDpi(top);
                    form.Left = screenResolution.ConvertXDpi(left + taskbarWidth);
                    form.Width = screenResolution.ConvertYDpi(width - taskbarWidth);
                    break;

                case TaskBar.TaskBarEdge.Top:
                    form.Top = screenResolution.ConvertYDpi(top + taskbarHeight);
                    form.Left = screenResolution.ConvertXDpi(left);
                    form.Width = screenResolution.ConvertXDpi(width);
                    break;

                case TaskBar.TaskBarEdge.Right:
                    form.Top = screenResolution.ConvertYDpi(top);
                    form.Left = screenResolution.ConvertXDpi(left);
                    form.Width = screenResolution.ConvertXDpi(width - taskbarWidth);
                    break;

                case TaskBar.TaskBarEdge.Bottom:
                    form.Top = screenResolution.ConvertYDpi(top);
                    form.Left = screenResolution.ConvertXDpi(left);
                    form.Width = screenResolution.ConvertXDpi(width);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PostionRight()
        {
            Screen activeScreen = ActiveScreen;
            Screen taskBarScreen = Screen.FromHandle(TaskBar.GetHandle()); ;
            Size taskBarSize = TaskBar.GetTaskBarSize();

            var left = activeScreen.Bounds.Left + activeScreen.Bounds.Width - screenResolution.ConvertXToScreen(form.Width);
            var top = activeScreen.Bounds.Top;
            var height = activeScreen.Bounds.Height;

            var taskbarHeight = (activeScreen.DeviceEquals(taskBarScreen) ? taskBarSize.Height : 0);
            var taskbarWidth = (activeScreen.DeviceEquals(taskBarScreen) ? taskBarSize.Width : 0); 

            switch (TaskBar.GetTaskBarEdge())
            {
                case TaskBar.TaskBarEdge.Left:
                    form.Left = screenResolution.ConvertXDpi(left);
                    form.Top = screenResolution.ConvertYDpi(top);
                    form.Height = screenResolution.ConvertYDpi(height);
                    break;

                case TaskBar.TaskBarEdge.Top:
                    form.Left = screenResolution.ConvertXDpi(left);
                    form.Top = screenResolution.ConvertYDpi(top + taskbarHeight);
                    form.Height = screenResolution.ConvertYDpi(height - taskbarHeight);
                    break;

                case TaskBar.TaskBarEdge.Right:
                    form.Left = screenResolution.ConvertXDpi(left - taskbarWidth);
                    form.Top = screenResolution.ConvertYDpi(top);
                    form.Height = screenResolution.ConvertYDpi(height);
                    break;

                case TaskBar.TaskBarEdge.Bottom:
                    form.Left = screenResolution.ConvertXDpi(left);
                    form.Top = screenResolution.ConvertYDpi(top);
                    form.Height = screenResolution.ConvertYDpi(height - taskbarHeight);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        private void PostionBottom()
        {
            Screen activeScreen = ActiveScreen;
            Screen taskBarScreen = Screen.FromHandle(TaskBar.GetHandle()); ;
            Size taskBarSize = TaskBar.GetTaskBarSize();

            var left = activeScreen.Bounds.Left;
            var top = activeScreen.Bounds.Top + activeScreen.Bounds.Height - screenResolution.ConvertYToScreen(form.Height);
            var width = activeScreen.Bounds.Width;

            var taskbarHeight = (activeScreen.DeviceEquals(taskBarScreen) ? taskBarSize.Height : 0);
            var taskbarWidth = (activeScreen.DeviceEquals(taskBarScreen) ? taskBarSize.Width : 0); 

            switch (TaskBar.GetTaskBarEdge())
            {
                case TaskBar.TaskBarEdge.Left:
                    form.Top = screenResolution.ConvertYDpi(top);
                    form.Left = screenResolution.ConvertXDpi(left + taskbarWidth);
                    form.Width = screenResolution.ConvertXDpi(width - taskbarWidth);
                    break;

                case TaskBar.TaskBarEdge.Top:
                    form.Top = screenResolution.ConvertYDpi(top);
                    form.Left = screenResolution.ConvertXDpi(left);
                    form.Width = screenResolution.ConvertXDpi(taskbarWidth);
                    break;

                case TaskBar.TaskBarEdge.Right:
                    form.Top = screenResolution.ConvertYDpi(top);
                    form.Left = screenResolution.ConvertXDpi(left);
                    form.Width = screenResolution.ConvertXDpi(width - taskbarWidth);
                    break;

                case TaskBar.TaskBarEdge.Bottom:
                    form.Top = screenResolution.ConvertYDpi(top - taskbarHeight);
                    form.Left = screenResolution.ConvertXDpi(left);
                    form.Width = screenResolution.ConvertXDpi(width);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Event Handlers

        private void TimerTick(object sender, EventArgs e)
        {
            try
            {
                Point currentPosition = TaskBar.GetTaskBarLocation();
                Size currentSize = TaskBar.GetTaskBarSize();
                TaskBar.TaskBarEdge currentEdge = TaskBar.GetTaskBarEdge();

                if (lastLocation != currentPosition || lastEdge != currentEdge || lastSize != currentSize)
                {
                    lastLocation = currentPosition;
                    lastEdge = currentEdge;
                    lastSize = currentSize;

                    if (!isFirstRun)
                        Dock(DockedTo, ActiveScreenIndex);

                    isFirstRun = false;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
    }

    public static class ScreenExtension
    {
        public static bool DeviceEquals(this Screen screen, Screen compareTo)
        {
            return screen.DeviceName == compareTo.DeviceName;
        }
    }
}