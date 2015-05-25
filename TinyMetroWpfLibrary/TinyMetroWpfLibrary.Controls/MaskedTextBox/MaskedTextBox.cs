using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using TinyMetroWpfLibrary.Utility;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Text.RegularExpressions;
using TinyMetroWpfLibrary.LogUtil;

namespace TinyMetroWpfLibrary.Controls
{
    public class MaskedTextBox : TextBox
    {
        private static ILogService _logger = LogService.GetLogger(typeof(MaskedTextBox));
        public MaskedTextBox()
        {
            AllowedChar = null;
            isNumberOnly = false;
        }
        public string AllowedChar { get; set; }
        public bool isNumberOnly { get; set; }

        public static readonly RoutedEvent SearchClickEvent = EventManager.RegisterRoutedEvent("SearchClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MaskedTextBox));
        public event RoutedEventHandler SearchClick
        {
            add { AddHandler(SearchClickEvent, value); }
            remove { RemoveHandler(SearchClickEvent, value); }
        }

        public void OnSearchClicked()
        {
            RaiseEvent(new RoutedEventArgs(SearchClickEvent));
        }

        public static readonly RoutedEvent ClearClickEvent = EventManager.RegisterRoutedEvent("ClearClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MaskedTextBox));
        public event RoutedEventHandler ClearClick
        {
            add { AddHandler(ClearClickEvent, value); }
            remove { RemoveHandler(ClearClickEvent, value); }
        }

        public void OnClearClicked()
        {
            this.Focus();
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action)delegate 
            {
                RaiseEvent(new RoutedEventArgs(ClearClickEvent));
            });
            
        }


        public bool IsValidate
        {
            get { return (bool)GetValue(IsValidateProperty); }
            set { SetValue(IsValidateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsValidate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsValidateProperty =
            DependencyProperty.Register("IsValidate", typeof(bool), typeof(MaskedTextBox), new PropertyMetadata(true));
        public Visibility SearchButtonVisibility
        {
            get { return (Visibility)GetValue(SearchButtonVisibilityProperty); }
            set { SetValue(SearchButtonVisibilityProperty, value); }
        }

        

        // Using a DependencyProperty as the backing store for SearchButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchButtonVisibilityProperty =
            DependencyProperty.Register("SearchButtonVisibility", typeof(Visibility), typeof(MaskedTextBox), new PropertyMetadata(Visibility.Collapsed));


        public Style SearchButtonStyle
        {
            get { return (Style)GetValue(SearchButtonStyleProperty); }
            set { SetValue(SearchButtonStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchButtonStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchButtonStyleProperty =
            DependencyProperty.Register("SearchButtonStyle", typeof(Style), typeof(MaskedTextBox), new PropertyMetadata(null));

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right)
                return;

            if (isNumberOnly)
            {
                if (!Regex.IsMatch(e.Key.ToString(), "[0-9]") && (e.Key != Key.OemPeriod) && (e.Key != Key.OemMinus))
                {
                    e.Handled = true;
                }
            }
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                RaiseEvent(new RoutedEventArgs(SearchClickEvent));
            }
        }
        private static string _path = null;
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (CheckIsTouchOver())
            {
                ShowVirtualKeyBoard();
            }
        }

        protected override void OnTouchDown(TouchEventArgs e)
        {
            base.OnTouchDown(e);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action)delegate
            {
                ShowVirtualKeyBoard();
            });
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Validation.RemoveErrorHandler(this, Text_Error);   
            Validation.AddErrorHandler(this, Text_Error);
            Button clearButton =  GetTemplateChild("PART_ClearText") as Button;
            if (clearButton != null)
            {
                clearButton.Click += ClearButton_Click;
            }

            Button searchButton = GetTemplateChild("PART_SearchButton") as Button;
            if (searchButton != null)
            {
                searchButton.Click += OnSearchButton_Click;
            }
            
        }

        private void OnSearchButton_Click(object sender, RoutedEventArgs e)
        {
            OnSearchClicked();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.Clear();
            OnClearClicked();
        }

        private void Text_Error(object sender, ValidationErrorEventArgs e)
        {
            IsValidate = !Validation.GetHasError(this);
        }

        private bool CheckIsTouchOver()
        {
            IEnumerable<TouchDevice> touchDevices = this.TouchesOver;
            if (touchDevices == null)
            {
                return false;
            }
            foreach (var item in touchDevices)
            {
                return true;
            }
            return false;
        }

        private bool IsWindows8OrNewer()
        {
            var os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT &&
                                  (os.Version.Major > 6 || (os.Version.Major == 6 && os.Version.Minor >= 2));
        }
        private void ShowVirtualKeyBoard()
        {
            if (!this.IsFocused)
            {
                return;
            }

            if (!IsWindows8OrNewer())
            {
                return;
            }

            if (_path == null)
            {
                _path = System.Environment.GetEnvironmentVariable("SystemDrive") + @"\Program Files\Common Files\microsoft shared\ink\tabtip.exe";
            }


            if (System.IO.File.Exists(_path))
            {
                try
                {
                    Process.Start(_path);
                }
                catch (Exception ex)
                {
                    _logger.Error(null, ex);
                }
            }
            else
            {
                _logger.Info("MaskTextBox Path)" + _path + "is not exist");
            }
        }
    }
}
    