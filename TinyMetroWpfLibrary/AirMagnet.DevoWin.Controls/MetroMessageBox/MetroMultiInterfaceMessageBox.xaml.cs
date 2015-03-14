using System;
using System.Collections.Generic;
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
    /// Interaction logic for MetroMultiInterfaceMessageBox.xaml
    /// </summary>
    public partial class MetroMultiInterfaceMessageBox : Window
    {
        public MetroMultiInterfaceMessageBox(string message, List<string> interfaceStringList)
        {
            InitializeComponent();
            var dataContext = new { Message = message, InterfaceList = interfaceStringList };
            this.DataContext = dataContext;
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action)delegate
            {
                ArrangeWindow();
            });

            InvalidateArrange();
        }
             
        protected override void OnActivated(EventArgs e)
        {
            ArrangeWindow();
            base.OnActivated(e);
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
            this.Close();
            this.Owner = null;
        }
    }
}
