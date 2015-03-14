// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace TinyMetroWpfLibrary.FrameControls.WindowedControl
{
    public partial class WindowedItemsControl
    {

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs args)
        {
            InvalidateMeasure();
            SetVerticalOffsetFromSelectedIndex();
            base.OnItemsChanged(args);
        }

        protected override void OnItemsPanelChanged(ItemsPanelTemplate oldItemsPanel, ItemsPanelTemplate newItemsPanel)
        {
            // This is not the actual WrappableStackPanel that will be used as the ItemsPanel.
            // This is another instance, and I use the field only for existence, not reference.
            isWrappableStackPanel = (newItemsPanel.LoadContent() as WrappableStackPanel) != null;
            base.OnItemsPanelChanged(oldItemsPanel, newItemsPanel);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            numberItems = this.Items.Count;
            viewportHeight = constraint.Height;
            extentHeight = base.MeasureOverride(new Size(constraint.Width, Double.PositiveInfinity)).Height;
            SetVerticalOffsetFromSelectedIndex();

            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (isWrappableStackPanel)
            {
                return base.ArrangeOverride(arrangeBounds);
            }

            if (this.VisualChildrenCount > 0)
            {
                UIElement child = GetVisualChild(0) as UIElement;

                if (child != null)
                    child.Arrange(new Rect(0, -VerticalOffset, arrangeBounds.Width, extentHeight));
            }
            return arrangeBounds;
        }
    }
}
