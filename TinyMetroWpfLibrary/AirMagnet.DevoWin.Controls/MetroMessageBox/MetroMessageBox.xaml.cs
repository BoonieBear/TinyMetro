using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AirMagnet.AircheckWifiTester.Controls
{
    /// <summary>
    /// Interaction logic for MetroMessageBox.xaml
    /// </summary>
    public partial class MetroMessageBox : Window
    {
        private EventHandler _eventHandler;
        public MetroMessageBox(string message, string title, MessageBoxButton buttons)
        {
            InitializeComponent();
            var dataContext = new { Message = message, Title = title, Buttons = buttons };
            this.DataContext = dataContext;
            _eventHandler = new EventHandler(SystemEvents_DisplaySettingsChanged);
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += _eventHandler;
        }

   
        public MessageBoxResult Result { get; set; }
        protected override void OnActivated(EventArgs e)
        {
            ArrangeWindow();
            base.OnActivated(e);
        }
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action)delegate
            {
                ArrangeWindow();
            });
           
            InvalidateArrange();
        }
        public void ReleaseEventHandler()
        {
            if (_eventHandler != null)
            {
                Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= _eventHandler;
                _eventHandler = null;
            }
        }
        private void ArrangeWindow()
        {
            if (Owner != null)
            {
                if (Owner.WindowState == WindowState.Maximized)
                {
                    Left = 0;
                    Top = (Owner.ActualHeight - this.ActualHeight) / 2;
                    Width = Owner.ActualWidth;
                }
                else
                {
                    Left = Owner.Left + 1;
                    Top = Owner.Top + ((Owner.ActualHeight - this.ActualHeight) / 2);
                    Width = Owner.ActualWidth - 2;
                }
            }
        }

        private void OnButtonsClicked(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            if (bt != null)
            {
                switch (bt.Name)
                {
                    case "YesButton":
                        Result = MessageBoxResult.Yes;
                        break;
                    case "NoButton":
                        Result = MessageBoxResult.No;
                        break;
                    case "OKButton":
                        Result = MessageBoxResult.OK;
                        break;
                    case "CancelButton":
                        Result = MessageBoxResult.Cancel;
                        break;
                    default:
                        Result = MessageBoxResult.None;
                        break;
                }
            }
            this.Close();
            this.Owner = null;
        }
    }
}
