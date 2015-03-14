using System;
using System.Windows.Forms;
using System.Windows.Markup;
using TinyMetroWpfLibrary.Helper;
using TinyMetroWpfLibrary.ViewModel;

namespace TinyMetroWpfLibrary.Extensions
{
    [MarkupExtensionReturnType(typeof(Uri))] 
    public class MetroStylesAccordingToCurrentResolution : MarkupExtension
    {
        private readonly ScreenResolution screenResolution = new ScreenResolution();

        #region Overrides of MarkupExtension

        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension. 
        /// </summary>
        /// <returns>
        /// The object value to set on the property where the extension is applied. 
        /// </returns>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            try
            {
                if (ViewModelBase.IsDesignMode)
                    return new Uri("/TinyMetroWpfLibrary;component/Styles/MetroStyles.xaml", UriKind.RelativeOrAbsolute);

                var activeScreen = Screen.PrimaryScreen;
                var dpi = screenResolution.Xdpi;

                // Get the width and height, you might want to at least round these to a few values. 
                var height = activeScreen.Bounds.Height;
                string resourceName;

                // Use the smaller sizes, if the screen resolution is higher than 96 dpi (which is standard) or the height is smaller or equal to 768
                if (height <= 768 || dpi > 96)
                    resourceName = "/TinyMetroWpfLibrary;component/Styles/MetroStyles768.xaml";
                else
                    resourceName = "/TinyMetroWpfLibrary;component/Styles/MetroStyles.xaml";

                // Add the resource to the app. 
                return new Uri(resourceName, UriKind.RelativeOrAbsolute);
            }
            catch (Exception)
            {
                // Don't throw an exception at this place.
                // Use the default
                return new Uri("/TinyMetroWpfLibrary;component/Styles/MetroStyles.xaml", UriKind.RelativeOrAbsolute);
            }
        }

        #endregion
    }
}