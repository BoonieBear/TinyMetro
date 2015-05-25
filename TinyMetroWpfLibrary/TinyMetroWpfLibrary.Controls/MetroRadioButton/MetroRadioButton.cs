using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace TinyMetroWpfLibrary.Controls
{
    public class MetroRadioButton:RadioButton
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Grid rootGrid = GetTemplateChild("Root") as Grid;
            if (rootGrid != null)
            {
                rootGrid.MouseLeftButtonDown += rootGrid_MouseLeftButtonDown;
            }
        }

        private void rootGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.IsChecked = !this.IsChecked;
        }
    }
}
