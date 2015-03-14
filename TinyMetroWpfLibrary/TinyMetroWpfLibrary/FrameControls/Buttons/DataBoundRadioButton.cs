// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;

namespace TinyMetroWpfLibrary.FrameControls.Buttons
{
    public class DataBoundRadioButton : RadioButton
    {
        protected override void OnChecked(RoutedEventArgs e)
        {
            // Do nothing. This will prevent IsChecked from being manually set and overwriting the binding.
        }

        protected override void OnToggle()
        {
            // Do nothing. This will prevent IsChecked from being manually set and overwriting the binding.
        }
    }
}
