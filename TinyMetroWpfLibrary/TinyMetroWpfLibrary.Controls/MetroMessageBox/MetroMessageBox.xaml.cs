using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TinyMetroWpfLibrary.Controls.MetroMessageBox
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
