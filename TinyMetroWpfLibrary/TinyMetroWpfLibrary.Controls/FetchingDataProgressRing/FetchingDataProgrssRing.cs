using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using System.Windows.Threading;

namespace TinyMetroWpfLibrary.Controls
{
    public class FetchingDataProgrssRing : Control
    {
        public ListView TargetListView
        {
            get { return (ListView)GetValue(TargetListViewProperty); }
            set { SetValue(TargetListViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetListView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetListViewProperty =
            DependencyProperty.Register("TargetListView", typeof(ListView), typeof(FetchingDataProgrssRing), new PropertyMetadata(null, TargetChanged));

        private static void TargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FetchingDataProgrssRing control = d as FetchingDataProgrssRing;
            if (control != null)
            {
                control.OnTargetChanged(e.NewValue as ListView);
            }
        }
        private DispatcherTimer _timer;
        public void OnTargetChanged(ListView listView)
        {
            if (listView != null)
            {
                DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(ListView.ItemsSourceProperty, typeof(ListView));
                descriptor.AddValueChanged(listView, new EventHandler(OnItemsSourceChanged));
                _timer = new DispatcherTimer(TimeSpan.FromSeconds(7),
                                                      DispatcherPriority.SystemIdle, (s, e) =>
                                                      {
                                                          this.Visibility = System.Windows.Visibility.Collapsed;
                                                          _timer.Stop();
                                                      },
                                                      Dispatcher.CurrentDispatcher);
            }
        }

        private  void OnItemsSourceChanged(object sender, EventArgs e)
        {
            if (TargetListView == null)
            {
                return;
            }

            if (TargetListView.ItemsSource == null)
            {
                return;
            }

            int count = 0;
            foreach (var item in TargetListView.ItemsSource)
            {
                ++count;
                break;
            }
            if (count >0)
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
            }
        }



        public int ItemsSourceCount
        {
            get { return (int)GetValue(ItemsSourceCountProperty); }
            set { SetValue(ItemsSourceCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSourceCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceCountProperty =
            DependencyProperty.Register("ItemsSourceCount", typeof(int), typeof(FetchingDataProgrssRing), new PropertyMetadata(0, ItemsSourceCountChanged));

        private static void ItemsSourceCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FetchingDataProgrssRing control = d as FetchingDataProgrssRing;
            if (control != null)
            {
                control.OnItemsSourceCountChanged((int)e.NewValue);
            }
        }

        public void OnItemsSourceCountChanged(int count)
        {
            if (count == 0)
            {
                if (this.Visibility == System.Windows.Visibility.Visible)
                {
                    _timer = new DispatcherTimer(TimeSpan.FromSeconds(7),
                    DispatcherPriority.SystemIdle, (s, e) =>
                    {
                        this.Visibility = System.Windows.Visibility.Collapsed;
                        _timer.Stop();
                    },
                    Dispatcher.CurrentDispatcher);
                }
            }
            else
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ProgressRing ring = GetTemplateChild("Part_ProgressRing") as ProgressRing;
            Canvas rootCanvas = GetTemplateChild("RootCanvas") as Canvas;
            if (ring != null && rootCanvas != null)
            {
                ring.Loaded+= (s,e)=>
                {
                    Window window =  TreeHelper.TryFindParent<Window>(this);
                    if (window != null)
                    {
                        //ring.Margin = new Thickness(0, (window.ActualHeight - 50)/2, 0, 0);
                        double screenY = (window.ActualHeight - 50) / 2;
                        double screenX = (window.ActualWidth - 50) / 2;
                        GeneralTransform trans = window.TransformToDescendant(rootCanvas);
                        Point canvasPoint = trans.Transform(new Point(screenX, screenY));
                        Canvas.SetLeft(ring, canvasPoint.X);
                        Canvas.SetTop(ring, canvasPoint.Y);
                    }
                };
                
            }
        }
        
    }
}
