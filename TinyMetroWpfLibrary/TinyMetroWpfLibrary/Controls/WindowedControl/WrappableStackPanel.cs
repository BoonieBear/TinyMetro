// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;

namespace BoonieBear.TinyMetro.WPF.Controls.WindowedControl
{
    /// <summary>
    /// The WrappableStackPanel is an extension to the Panel that is used as the base for all Picker classes.
    /// Implementation is based on articel of Charles Petzold: <see cref="http://msdn.microsoft.com/de-de/magazine/gg309180.aspx"/>
    /// </summary>
    public class WrappableStackPanel : Panel
    {
        double itemsHeight;

        public static readonly DependencyProperty StartIndexProperty =
            WindowedItemsControl.StartIndexProperty.AddOwner(typeof(WrappableStackPanel),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange));

        public double StartIndex
        {
            set { SetValue(StartIndexProperty, value); }
            get { return (double)GetValue(StartIndexProperty); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size compositeSize = new Size();

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(new Size(availableSize.Width, Double.PositiveInfinity));
                compositeSize.Width = Math.Max(compositeSize.Width, child.DesiredSize.Width);
                compositeSize.Height += child.DesiredSize.Height;
            }

            if (InternalChildren.Count == 0)
                itemsHeight = 0;
            else
                itemsHeight = compositeSize.Height / InternalChildren.Count;

            return compositeSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (InternalChildren.Count == 0)
                return base.ArrangeOverride(finalSize);

            int count = InternalChildren.Count;
            int intStartIndex = (int)StartIndex;
            double y = -(StartIndex % 1) * itemsHeight;

            for (int index = 0; index < count; index++)
            {
                UIElement child = InternalChildren[(intStartIndex + index) % count];
                child.Arrange(new Rect(new Point(0, y), new Size(finalSize.Width, child.DesiredSize.Height)));
                y += child.DesiredSize.Height;
            }
            
            return base.ArrangeOverride(finalSize);
        }
    }
}
