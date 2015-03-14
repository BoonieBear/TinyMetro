using System;
using System.Windows;
using System.Windows.Controls;

namespace TinyMetroWpfLibrary.Controls.Tile
{
    public class Tile : ContentControl
    {

        private const string NormalState = "Normal";
        private const string PressedState = "Pressed";
       
        public static readonly RoutedEvent DoubleClickEvent = EventManager.RegisterRoutedEvent("DoubleClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Tile));
        public event RoutedEventHandler DoubleClick
        {
            add { AddHandler(DoubleClickEvent, value); }
            remove { RemoveHandler(DoubleClickEvent, value); }
        }

        private const int MAX_DOUBLECLICK_TIMESPAN = 500;
        private DateTime _ClickTime;
        public DateTime ClickTime
        {
            get { return _ClickTime; }
            set
            {
                TimeSpan timeSpan = value - _ClickTime;
                if (timeSpan.TotalMilliseconds < MAX_DOUBLECLICK_TIMESPAN)
                {
                    RaiseEvent(new RoutedEventArgs(DoubleClickEvent));
                }
                _ClickTime = value;
                 
            }
        }
        
        static Tile()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Tile), new FrameworkPropertyMetadata(typeof(Tile)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PreviewMouseLeftButtonDown += (sender, e) => 
            {

                ClickTime = DateTime.Now; 
                VisualStateManager.GoToState(this, PressedState, true); 
            };
            this.PreviewMouseLeftButtonUp += (sender, e) => { VisualStateManager.GoToState(this, NormalState, true); };
            VisualStateManager.GoToState(this, "Normal", false);
        }
    }
}
