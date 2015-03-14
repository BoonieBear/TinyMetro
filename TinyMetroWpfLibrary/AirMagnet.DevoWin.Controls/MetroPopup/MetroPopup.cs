using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AirMagnet.AircheckWifiTester.Controls
{
    public class MetroPopup : Popup
    {
        public ButtonBase TargetButton
        {
            get { return (ButtonBase)GetValue(TargetButtonProperty); }
            set { SetValue(TargetButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetButtonProperty =
            DependencyProperty.Register("TargetButton", typeof(ButtonBase), typeof(MetroPopup), new PropertyMetadata(null, TargetButtonChanged));

        private static void TargetButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MetroPopup metroPopup = d as MetroPopup;
            if (metroPopup != null)
            {
                metroPopup.OnTargetButtonChanged(e.NewValue as ButtonBase);
            }
        }

        public void OnTargetButtonChanged(ButtonBase button)
        {
            if (button != null)
            {
                button.Click += (s, e) => { this.IsOpen = true; };
                //UserControl window = TreeHelper.TryFindParent<UserControl>(button);
                //if (window == null)
                //{
                //    return;
                //}
                //window.PreviewMouseLeftButtonDown += (s, e) => 
                //{
                //    if (this.IsOpen && !this.IsMouseOver)
                //    {
                //        e.Handled = true;
                //        this.IsOpen = false;
                //    }
                //};
            }
        }
        
    }
}
