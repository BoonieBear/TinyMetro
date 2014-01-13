// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BoonieBear.TinyMetro.WPF.Controls.ErrorBar
{
    /// <summary>
    /// Interaction logic for DisplayErrorBar.xaml
    /// </summary>
    public partial class DisplayErrorBar : UserControl, INotifyPropertyChanged
    {
        // Property Changed Event
        public event PropertyChangedEventHandler PropertyChanged;

        #region DependencyProperties

        /// <summary>
        /// Command DependencyProeprty
        /// </summary>
        public static DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(DisplayErrorBar));

        /// <summary>
        /// Text Dependency Property
        /// </summary>
        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DisplayErrorBar));

        #endregion

        /// <summary>
        /// Initializes a new ErrorBar
        /// </summary>
        public DisplayErrorBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the text of the ErrorBar
        /// </summary>
        [Category("Behavior"), DefaultValue(""), Description("Error Message that will be displayed"), NotifyParentProperty(true)]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set
            {
                if (Text == value)
                    return;

                SetValue(TextProperty, value);
                OnNotifyPropertyChanged("Text");
            }
        }

        /// <summary>
        /// Gets or sets a command that will be fired, when the command of the ErrorBar has been pressed
        /// </summary>
        [Category("Behavior"), Description("Command bound to the DisplayError bar"), NotifyParentProperty(true)]
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
