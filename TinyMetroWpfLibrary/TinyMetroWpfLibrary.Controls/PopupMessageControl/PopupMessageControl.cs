using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace TinyMetroWpfLibrary.Controls
{
    public class PopupMessageControl : ContentControl
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Rectangle backgrounRect = GetTemplateChild("BackgroundRectangle") as Rectangle;
            if (backgrounRect != null)
            {
                backgrounRect.MouseLeftButtonDown += (s, e) =>
                {
                    MediaManager.Instance.PlayBackgroundMedia();
                };
            }
        }


        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(PopupMessageControl), new PropertyMetadata(false));

        
    }
}
