using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TinyMetroWpfLibrary.Controls.Utils;

namespace TinyMetroWpfLibrary.Controls.PercentageProgressRing
{
    public class PercentageProgressRing : Control
    {

        public int Percentange
        {
            get { return (int)GetValue(PercentangeProperty); }
            set { SetValue(PercentangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Percentange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PercentangeProperty =
            DependencyProperty.Register("Percentange", typeof(int), typeof(PercentageProgressRing), new PropertyMetadata(0));



        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(PercentageProgressRing), new PropertyMetadata(false));



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(PercentageProgressRing), new PropertyMetadata(null));



        public bool IsSupportPercentage
        {
            get { return (bool)GetValue(IsSupportPercentageProperty); }
            set { SetValue(IsSupportPercentageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSupportPercentage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSupportPercentageProperty =
            DependencyProperty.Register("IsSupportPercentage", typeof(bool), typeof(PercentageProgressRing), new PropertyMetadata(true));

        
        
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //Grid contentGrid = GetTemplateChild("ContentGrid") as Grid;
            Canvas rootCanvas = GetTemplateChild("RootCanvas") as Canvas;
            StackPanel contentStackPanel = GetTemplateChild("ContentStackPanel") as StackPanel;
            if (contentStackPanel != null && rootCanvas != null)
            {
                contentStackPanel.Loaded += (s, e) =>
                {
                    Window window = TreeHelper.TryFindParent<Window>(this);
                    if (window != null)
                    {
                        double screenY = (window.ActualHeight - contentStackPanel.ActualHeight) / 2;
                        double screenX = (window.ActualWidth - contentStackPanel.ActualWidth) / 2;
                        GeneralTransform trans = window.TransformToDescendant(rootCanvas);
                        Point canvasPoint = trans.Transform(new Point(screenX, screenY));
                        Canvas.SetLeft(contentStackPanel, canvasPoint.X);
                        Canvas.SetTop(contentStackPanel, canvasPoint.Y);
                    }
                };

            }
        }
    }
}
