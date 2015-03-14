using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace TinyMetroWpfLibrary.Controls.PopupMessageControl
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
                    MediaManager.MediaManager.Instance.PlayBackgroundMedia();
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
