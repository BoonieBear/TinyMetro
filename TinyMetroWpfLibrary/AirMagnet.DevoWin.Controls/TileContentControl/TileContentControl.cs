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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AirMagnet.AircheckWifiTester.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:AirMagnet.AircheckWifiTester.Controls.TileContentControl"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:AirMagnet.AircheckWifiTester.Controls.TileContentControl;assembly=AirMagnet.AircheckWifiTester.Controls.TileContentControl"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:TileContentControl/>
    ///
    /// </summary>
    public class TileContentControl : ContentControl
    {
        static TileContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TileContentControl), new FrameworkPropertyMetadata(typeof(TileContentControl)));
        }


        #region propertys
        public HorizontalAlignment TextHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(TextHorizontalAlignmentProperty); }
            set { SetValue(TextHorizontalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextHorizontalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextHorizontalAlignmentProperty =
            DependencyProperty.Register("TextHorizontalAlignment", typeof(HorizontalAlignment), typeof(TileContentControl), new PropertyMetadata(HorizontalAlignment.Center));


        public Brush IconBackground
        {
            get { return (Brush)GetValue(IconBackgroundProperty); }
            set { SetValue(IconBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconBackgroundProperty =
            DependencyProperty.Register("IconBackground", typeof(Brush), typeof(TileContentControl), new PropertyMetadata(null));


        public Brush TileBackground
        {
            get { return (Brush)GetValue(TileBackgroundProperty); }
            set { SetValue(TileBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TileBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TileBackgroundProperty =
            DependencyProperty.Register("TileBackground", typeof(Brush), typeof(TileContentControl), new PropertyMetadata(null));
        #endregion

        #region event
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TileContentControl));
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }
        #endregion

        #region helper
        public override void OnApplyTemplate()
        {
            Button bt = GetTemplateChild("PARTS_EventButton") as Button;
            if (bt != null)
            {
                bt.Click += new RoutedEventHandler((sender,e)=>
                {
                    base.RaiseEvent(new RoutedEventArgs(ClickEvent));
                });
            }
        }
        #endregion
    }
}
