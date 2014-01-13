// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BoonieBear.TinyMetro.WPF.Controls.AppBar
{
    /// <summary>
    /// Defines an application bar icon
    /// </summary>
    public class ApplicationBarIcon : FrameworkElement, INotifyPropertyChanged
    {
        // Property Changed Event
        public event PropertyChangedEventHandler PropertyChanged;

        #region DependencyProperties

        /// <summary>
        /// Command DependencyProeprty
        /// </summary>
        public static DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof (ICommand), typeof (ApplicationBarIcon));

        /// <summary>
        /// Description Dependency Property
        /// </summary>
        public static DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(ApplicationBarIcon));

        /// <summary>
        /// Image Source Dependency Property
        /// </summary>
        public static DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ApplicationBarIcon));

        #endregion

        /// <summary>
        /// Gets or sets the image source 
        /// </summary>
        [Category("Behavior"), DefaultValue(""), Description("Image source of the application bar icon"), NotifyParentProperty(true)]
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set
            {
                if (ImageSource == value)
                    return;
                
                SetValue(ImageSourceProperty, value);
                OnNotifyPropertyChanged("ImageSource");
            }
        }

        /// <summary>
        /// Gets or sets the application bar icon description
        /// </summary>
        [Category("Behavior"), DefaultValue(""), Description("Description of the application bar icon"), NotifyParentProperty(true)]
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set
            {
                if (Description == value)
                    return;

                SetValue(DescriptionProperty, value);
                OnNotifyPropertyChanged("Description");
            }
        }

        /// <summary>
        /// Gets or sets a command that will be fired, when an applciation bar icon has been pressed
        /// </summary>
        [Category("Behavior"), Description("Command bound to the application bar icon"), NotifyParentProperty(true)]
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set
            {
                if (Command == value)
                    return;

                SetValue(CommandProperty, value);
                OnNotifyPropertyChanged("Command");
            }
        }

        /// <summary>
        /// Notify on property change
        /// </summary>
        /// <param name="property">name of the property</param>
        private void OnNotifyPropertyChanged(string property)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
