using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BoonieBear.TinyMetro.WPF.Resources;
using BoonieBear.TinyMetro.WPF.ViewModel;
using BoonieBear.TinyMetro.WPF.Controls.AppBar;

namespace BoonieBear.TinyMetro.WPF.Controls.TextBox
{
    public class ExtendedTextBoxViewModel : ViewModelBase
    {
        /// <summary>
        /// Referenced TextBox
        /// </summary>
        private readonly ExtendedTextBox textBox;

        /// <summary>
        /// Initializes a new instance of the ExtendedTextBoxViewModel class
        /// </summary>
        public ExtendedTextBoxViewModel(ExtendedTextBox box)
        {
            textBox = box;
            ApplyCommand = RegisterCommand(ExecuteApplyCommand, CanExecuteApplyCommand, true);
            CancelCommand = RegisterCommand(ExecuteCancelCommand, CanExecuteCancelCommand, true);
        }

        /// <summary>
        /// Initializes the ViewModel
        /// </summary>
        public override void Initialize()
        {
        }

        /// <summary>
        /// Initializes the page.
        /// This method will be called every time the user navigates to the page
        /// </summary>
        /// <param name="extraData">The extra Data, if there's any. Otherwise NULL</param>
        public override void InitializePage(object extraData)
        {
        }

        #region Apply Command

        /// <summary>
        /// Gets or sets the Apply command.
        /// </summary>
        /// <value>The Apply command.</value>
        public ICommand ApplyCommand
        {
            get { return GetPropertyValue(() => ApplyCommand); }
            set { SetPropertyValue(() => ApplyCommand, value); }
        }

        /// <summary>
        /// Determines whether this instance can execute Apply command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute Apply command; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Event arguments</param>
        public void CanExecuteApplyCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = textBox != null;
        }

        /// <summary>
        /// Executes the Apply command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Event arguments</param>
        public void ExecuteApplyCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            textBox.HideApplicationBarIcons(textBox.ApplicationBar);
        }

        #endregion

        #region Cancel Command

        /// <summary>
        /// Gets or sets the Cancel command.
        /// </summary>
        /// <value>The Cancel command.</value>
        public ICommand CancelCommand
        {
            get { return GetPropertyValue(() => CancelCommand); }
            set { SetPropertyValue(() => CancelCommand, value); }
        }

        /// <summary>
        /// Determines whether this instance can execute Cancel command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute Cancel command; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Event arguments</param>
        public void CanExecuteCancelCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = textBox != null;
        }

        /// <summary>
        /// Executes the Cancel command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Event arguments</param>
        public void ExecuteCancelCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            textBox.RecoverText();
            textBox.HideApplicationBarIcons(textBox.ApplicationBar);
        }

        #endregion
    }

    /// <summary>
    /// The extended textbox can be used to show application bar button
    /// </summary>
    public class ExtendedTextBox : System.Windows.Controls.TextBox, INotifyPropertyChanged
    {
        private readonly ApplicationBarIcon applyButton;
        private readonly ApplicationBarIcon cancelButton;
        private readonly ExtendedTextBoxViewModel boxViewModel;

        private ApplicationBarIcon[] storedIcons;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedTextBox"/> class.
        /// </summary>
        public ExtendedTextBox()
        {
            DependencyPropertyDescriptor.FromProperty(IsReadOnlyProperty, typeof(ExtendedTextBox)).AddValueChanged(this, (s, e) => NotifyPropertyChanged("IsEmpty"));
            DependencyPropertyDescriptor.FromProperty(TextProperty, typeof(ExtendedTextBox)).AddValueChanged(this, (s, e) => NotifyPropertyChanged("IsEmpty"));

            boxViewModel = new ExtendedTextBoxViewModel(this);

            KeyUp += OnReturnHandling;
            GotFocus += OnTextBoxGotFocus;
            LostFocus += OnTextBoxLostFocus;

            applyButton = new ApplicationBarIcon
                              {
                                  ImageSource = new BitmapImage(new Uri("/TinyMetroWpfLibrary;component/Images/appbar.check.png", UriKind.RelativeOrAbsolute)),
                                  Description = CommonResources.Button_Ok,
                                  DataContext = boxViewModel,
                                  Command = boxViewModel.ApplyCommand,
                                  Focusable = false
                              };

            cancelButton = new ApplicationBarIcon
                               {
                                   ImageSource = new BitmapImage(new Uri("/TinyMetroWpfLibrary;component/Images/appbar.cancel.png", UriKind.RelativeOrAbsolute)),
                                   Description = CommonResources.Button_Cancel,
                                   DataContext = boxViewModel,
                                   Command = boxViewModel.CancelCommand,
                                   Focusable = false
                               };

            IsReadOnly = true;
        }

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Returns true, if the empty text shall be sown
        /// </summary>
        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Text) && IsReadOnly; }
        }

        // Empty Text Dependency Property
        public static readonly DependencyProperty EmptyTextProperty =
            DependencyProperty.Register("EmptyText", typeof (string), 
            typeof (ExtendedTextBox), null);

        /// <summary>
        /// Gets or sets the Empty Text
        /// </summary>
        public string EmptyText
        {
            get { return (string) GetValue(EmptyTextProperty); }
            set { SetValue(EmptyTextProperty, value); }
        }

        // Application Bar Dependency Property
        public static readonly DependencyProperty PageProperty =
             DependencyProperty.Register("ApplicationBar", typeof(ApplicationBar),
             typeof(ExtendedTextBox), null);

        /// <summary>
        /// Gets the Application Bar
        /// </summary>
        /// <value>application Bar of the page</value>
        public ApplicationBar ApplicationBar
        {
            get { return (ApplicationBar)GetValue(PageProperty); }
            set { SetValue(PageProperty, value); }
        }

        /// <summary>
        /// Called when the user presses the enter key in order to apply the content
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        protected void OnReturnHandling(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            // Move the focus to the next, when return key has been pressed
            if (ApplicationBar != null && !AcceptsReturn)
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        /// <summary>
        /// Called when the textbox gains it's focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (!IsReadOnly)
                return;

            Tag = Text;

            // what to do on get focus?
            ShowApplicationBarIcons(ApplicationBar);
        }

        /// <summary>
        /// Called when the textbox looses it's focus. In that case, the application bar icons need to be updated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
                return;

            Text = Text.Trim();
            var appBar = ApplicationBar;
            if (appBar == null) return;

            HideApplicationBarIcons(ApplicationBar);
        }

        /// <summary>
        /// Shows the Application Bar Icons
        /// </summary>
        /// <param name="applicationBar">application bar of the page</param>
        internal void ShowApplicationBarIcons(ApplicationBar applicationBar)
        {
            if (applicationBar == null || storedIcons != null)
                return;

            // Store the current icons and reset all
            storedIcons = applicationBar.Icons.ToArray();
            applicationBar.Icons.Clear();
            applicationBar.Icons.Add(applyButton);
            applicationBar.Icons.Add(cancelButton);
            IsReadOnly = false;
        }

        /// <summary>
        /// Hides the Application Bar Icons
        /// </summary>
        /// <param name="applicationBar">application bar of the page</param>
        internal void HideApplicationBarIcons(ApplicationBar applicationBar)
        {
            if (applicationBar == null || storedIcons == null)
                return;

            // Restore the saved icons
            applicationBar.Icons.Clear();
            foreach (var barIcon in storedIcons)
                applicationBar.Icons.Add(barIcon);
            storedIcons = null;
            IsReadOnly = true;

            // Now move the focus away
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        /// <summary>
        /// Recovers the stored Text
        /// </summary>
        public void RecoverText()
        {
            var storedText = Tag as string;
            if (storedText == null)
                return;

            Text = storedText;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
