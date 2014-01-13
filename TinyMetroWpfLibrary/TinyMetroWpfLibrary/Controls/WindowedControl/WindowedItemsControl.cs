// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BoonieBear.TinyMetro.WPF.Controls.WindowedControl
{
    public partial class WindowedItemsControl: ItemsControl
    {
        bool isWrappableStackPanel;
        int numberItems;
        double itemsHeight;
        double viewportHeight;
        double extentHeight;

        public event DependencyPropertyChangedEventHandler IsActiveChanged;

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive",
                typeof(bool),
                typeof(WindowedItemsControl),
                new PropertyMetadata(false, OnIsActiveChanged));

        public static readonly DependencyProperty SelectedIndexProperty = 
            Selector.SelectedIndexProperty.AddOwner(
                typeof(WindowedItemsControl), 
                new FrameworkPropertyMetadata(-1));

        public static readonly DependencyProperty SelectedItemProperty =
            Selector.SelectedItemProperty.AddOwner(
                typeof(WindowedItemsControl),
                new FrameworkPropertyMetadata(null, OnSelectedItemChanged));

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset",
                typeof(double),
                typeof(WindowedItemsControl),
                new UIPropertyMetadata(OnVerticalOffsetChanged){IsAnimationProhibited = false});

        public static readonly DependencyProperty StartIndexProperty =
            DependencyProperty.RegisterAttached("StartIndex",
                typeof(double),
                typeof(WindowedItemsControl),
                new FrameworkPropertyMetadata(0.0, 
                    FrameworkPropertyMetadataOptions.Inherits));

        public WindowedItemsControl()
        {
            IsManipulationEnabled = true;
        }

        public bool IsActive
        {
            set { SetValue(IsActiveProperty, value); }
            get { return (bool)GetValue(IsActiveProperty); }
        }

        public int SelectedIndex
        {
            set
            {
                SetValue(SelectedIndexProperty, value);

                if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
                    SelectedItem = Items[SelectedIndex];
                else
                    SelectedItem = null;

                SetVerticalOffsetFromSelectedIndex();
            }

            get { return (int)GetValue(SelectedIndexProperty); }
        }

        public object SelectedItem
        {
            set { SetValue(SelectedItemProperty, value); }
            get { return GetValue(SelectedItemProperty); }
        }

        public double VerticalOffset
        {
            set { SetValue(VerticalOffsetProperty, value); }
            get { return (double)GetValue(VerticalOffsetProperty); }
        }

        public double StartIndex
        {
            set { SetValue(StartIndexProperty, value); }
            get { return (double)GetValue(StartIndexProperty); }
        }

        public bool IsManipulationInProgress
        {
            get { return manipulationInProgress; }
        }

        // IsActive property-changed handlers
        static void OnIsActiveChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((WindowedItemsControl) obj).OnIsActiveChanged(args);
        }

        void OnIsActiveChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsActiveChanged != null)
                IsActiveChanged(this, args);
        }

        // SelectedItem property-changed handlers
        static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((WindowedItemsControl) obj).OnSelectedItemChanged(args);
        }

        void OnSelectedItemChanged(DependencyPropertyChangedEventArgs args)
        {
            if (SelectedItem == null)
                SelectedIndex = -1;
            else
                SelectedIndex = Items.IndexOf(SelectedItem);
        }

        static void OnVerticalOffsetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((WindowedItemsControl) obj).OnVerticalOffsetChanged(args);
        }

        void OnVerticalOffsetChanged(DependencyPropertyChangedEventArgs args)
        {
            if (!isWrappableStackPanel)
            {
                InvalidateArrange();
            }
            else if (itemsHeight > 0 && numberItems > 0)
            {
                double startIndex = VerticalOffset / itemsHeight;

                startIndex %= numberItems;
                startIndex = (startIndex + numberItems) % numberItems;
                StartIndex = startIndex;
            }
        }

        void SetVerticalOffsetFromSelectedIndex()
        {
            numberItems = Items.Count;
            itemsHeight = numberItems > 0 ? extentHeight / numberItems : 0;

            if (SelectedIndex != -1 && !manipulationInProgress)
            {
                VerticalOffset = VerticalOffsetFromCenteredIndex(SelectedIndex);
            }
        }

        double VerticalOffsetFromCenteredIndex(int index)
        {
            return index * itemsHeight - (viewportHeight - itemsHeight) / 2;
        }

        int NormalizeIndex(int index)
        {
            if (numberItems == 0)
                return 0;

            while (index < 0)
                index += numberItems;

            while (index >= numberItems)
                index -= numberItems;

            return index;
        }

        // not used yet
        double NormalizeIndex(double index)
        {
            if (numberItems == 0)
                return 0;

            while (index < 0)
                index += numberItems;

            while (index >= numberItems)
                index -= numberItems;

            return index;
        }
        
        double FractionalCenteredIndexFromVerticalOffset(double verticalOffset, bool normalize)
        {
            double index = (verticalOffset + (viewportHeight - itemsHeight) / 2) / itemsHeight;

            if (normalize)
            {
                index %= numberItems;
                index = (index + numberItems) % numberItems;
            }

            return index;
        }
    }
}
