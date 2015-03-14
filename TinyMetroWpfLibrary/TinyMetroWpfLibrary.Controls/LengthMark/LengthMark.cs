﻿using System.Windows;
using System.Windows.Controls;

namespace TinyMetroWpfLibrary.Controls.LengthMark
{
    public class LengthMark : ContentControl 
    {
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(LengthMark), new PropertyMetadata(Orientation.Horizontal));
    }
}
