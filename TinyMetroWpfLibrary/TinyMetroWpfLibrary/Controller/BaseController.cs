// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using TinyMetroWpfLibrary.EventAggregation;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.FrameControls.Picker;
using TinyMetroWpfLibrary.Frames;
using TinyMetroWpfLibrary.Helper;
using TinyMetroWpfLibrary.ViewModel;

namespace TinyMetroWpfLibrary.Controller
{
    /// <summary>
    /// Base class for module specific controllers
    /// </summary>
    public abstract class BaseController :
        IHandleMessage<GoBackNavigationRequest>,
        IHandleMessage<CloseApplicationCommand>,
        IHandleMessage<ListPickerFullModeRequest>,
        IHandleMessage<DatePickerFullModeRequest>,
        IHandleMessage<TimePickerFullModeRequest>,
        IHandleMessage<TimeSpanPickerFullModeRequest>,
        IHandleMessage<WindowStateApplicationCommand>,
        IHandleMessage<ForceBindingUpdateEvent>,
        IHandleMessage<ChangeAnimationModeRequest>, 
        INavigationController
    {
        private NavigationMode navigationMode;
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the BaseController class.
        /// </summary>
        protected BaseController()
        {
            EventAggregator = Kernel.Instance.EventAggregator;
            EventAggregator.Subscribe(this);
        }

        /// <summary>
        /// This method can do some initializations
        /// </summary>
        public virtual void Init()
        {
        }

        /// <summary>
        /// Handles a message of a specific type.
        /// </summary>
        /// <param name="message">the message to handle</param>
        public virtual void Handle(ForceBindingUpdateEvent message)
        {
            // Check, if we need to dispatch
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                var waitHandle = new AutoResetEvent(false);
                Application.Current.Dispatcher.Invoke((Action)(
                    () =>
                    {
                        Handle(message);
                        waitHandle.Set();
                    }));

                waitHandle.WaitOne();
                return;
            }

            var topWindow = Application.Current.Windows.Count - 1;
            var element = Application.Current.Windows[topWindow];
            if (element == null)
                return;

            var focusObj = FocusManager.GetFocusedElement(element);
            if (!(focusObj is TextBox))
                return;

            var binding = (focusObj as TextBox).GetBindingExpression(TextBox.TextProperty);

            if (binding != null)
                binding.UpdateSource();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="BaseController"/> is reclaimed by garbage collection.
        /// </summary>
        ~BaseController()
        {
            EventAggregator.Unsubscribe(this);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Attaches the controller to the root frame
        /// </summary>
        /// <param name="contentFrame">The Content Frame</param>
        public virtual void SetRootFrame(Frame contentFrame)
        {
            if (contentFrame != null)
            {
                // wire events
                contentFrame.Navigated += OnNavigated;
                contentFrame.Navigating += OnNavigating;
            }

            ContentFrame = contentFrame;
        }

        /// <summary>
        /// Gets or sets the content frame
        /// </summary>
        protected Frame ContentFrame { get; set; }

        /// <summary>
        /// Called when the NavigationService navigates to a new page inside the application
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Navigating Event Args</param>
        protected virtual void OnNavigated(object sender, NavigationEventArgs e)
        {
            var page = e.Content as Page;

            // keep track of the current page
            CurrentPage = page;
            if (page == null) return;

            // Send NavigationCompletedEvent, if it's a normal navigation request
            object context = page.DataContext;
            if (context != null)
            {
                // Send Navigated Request
                Type requestType = typeof (NavigationCompletedEvent<>).MakeGenericType(new[] {context.GetType()});

                MethodInfo publishMessage = EventAggregator.GetType().GetMethod("PublishMessage");
                publishMessage = publishMessage.GetGenericMethodDefinition().MakeGenericMethod(new[] {requestType});
                publishMessage.Invoke(EventAggregator, new[] { Activator.CreateInstance(requestType, navigationMode) });
            }

            // Initializes the Page if the DataContext is derived from the ViewModelBase
            var viewModelBase = page.DataContext as ViewModelBase;
            if (viewModelBase != null)
                viewModelBase.InitializePage(e.ExtraData);

            EventAggregator.PublishMessage(new NavigatedEvent());
        }

        /// <summary>
        /// On Navigation Handler
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Navigating Cancel Event Args</param>
        protected virtual void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            navigationMode = e.NavigationMode;
            EventAggregator.PublishMessage(new NavigationEvent(e));
        }

        /// <summary>
        /// Navigates to page.
        /// </summary>
        /// <param name="navigateToPage">The navigate to page.</param>
        public virtual void NavigateToPage(string navigateToPage)
        {
            if (CurrentPage == null)
                Application.Current.Dispatcher.BeginInvoke((ThreadStart)(() =>
                    ContentFrame.Navigate(new Uri(navigateToPage, UriKind.Relative))));
            else
            {
                if (CurrentPage.Dispatcher.CheckAccess())
                {
                    if (CurrentPage.NavigationService == null)
                        return;

                    PasswordBoxAssistant.IsNavigating = true;
                    try
                    {
                        CurrentPage.NavigationService.Navigate(new Uri(navigateToPage, UriKind.Relative));
                    }
                    finally
                    {
                        PasswordBoxAssistant.IsNavigating = false;
                    }
                }
                else
                    CurrentPage.Dispatcher.BeginInvoke((ThreadStart) (() => NavigateToPage(navigateToPage)));
            }
        }

        /// <summary>
        /// Navigates to a page and add additional data
        /// </summary>
        /// <param name="navigateToPage">The navigate to page.</param>
        /// <param name="message">The message thas will be send to the page</param>
        public virtual void NavigateToPage(string navigateToPage, object message)
        {
            if (CurrentPage == null)
                Application.Current.Dispatcher.BeginInvoke((ThreadStart)(() => 
                    ContentFrame.Navigate(new Uri(navigateToPage, UriKind.Relative), message)));
            else
            {
                if (CurrentPage.Dispatcher.CheckAccess())
                {
                    if (CurrentPage.NavigationService == null)
                        return;

                    PasswordBoxAssistant.IsNavigating = true;
                    try
                    {
                        CurrentPage.NavigationService.Navigate(new Uri(navigateToPage, UriKind.Relative), message);
                    }
                    finally
                    {
                        PasswordBoxAssistant.IsNavigating = false;    
                    }
                }
                else
                    CurrentPage.Dispatcher.BeginInvoke((ThreadStart) (() => NavigateToPage(navigateToPage, message)));
            }
        }

        /// <summary>
        /// Determines whether this instance [can go back].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance [can go back]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanGoBack
        {
            get
            {
                // Maybe we can't access it directly
                if (CurrentPage != null && !CurrentPage.CheckAccess())
                {
                    bool result = false;
                    CurrentPage.Dispatcher.Invoke((Action)(() => result = CanGoBack));
                    return result;
                }

                return CurrentPage != null && CurrentPage.NavigationService != null && CurrentPage.NavigationService.CanGoBack;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the controller that used for monitor the navigational messages
        /// </summary>
        protected IEventAggregator EventAggregator { get; set; }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>The current page.</value>
        public Page CurrentPage { get; private set; }

        #endregion Properties

        #region IHandleMessage<GoBackNavigationRequest> Members

        /// <summary>
        /// Handles the Navigation Request
        /// </summary>
        /// <param name="message">The Navigation goback request</param>
        public virtual void Handle(GoBackNavigationRequest message)
        {
            // Check, if we can go back
            if (!CanGoBack)
                return;

            // Now Go Back
            if (CurrentPage.Dispatcher.CheckAccess())
            {
                // When going back, delete an potential error and clear the state
                var baseModel = CurrentPage.DataContext as ViewModelBase;
                if (baseModel != null && baseModel.IsDefective)
                    baseModel.Error = string.Empty;

                if (CurrentPage.NavigationService == null)
                    return;

                if (CurrentPage.NavigationService.CanGoBack)
                    CurrentPage.NavigationService.GoBack();
            }
            else
                CurrentPage.Dispatcher.BeginInvoke((ThreadStart)(() => Handle(message)));
        }

        #endregion

        #region IHandleMessage<CloseApplicationCommand>

        /// <summary>
        /// Handles a message of a specific type.
        /// </summary>
        /// <param name="message">the message to handle</param>
        public virtual void Handle(CloseApplicationCommand message)
        {
            // Maybe we can't access it directly
            if (!Application.Current.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke((Action) (() => Handle(message)));
                return;
            }

            ForceClosing = message.ForceClosing;
            Application.Current.MainWindow.Close();
        }

        /// <summary>
        /// Gets a value indicating whether the closing of the current window shall be forced.
        /// </summary>
        public virtual bool ForceClosing { get; private set; }

        #endregion

        #region IHandleMessage<WindowStateApplicationCommand>

        /// <summary>
        /// Handles a message of a specific type.
        /// </summary>
        /// <param name="message">the message to handle</param>
        public virtual void Handle(WindowStateApplicationCommand message)
        {
            // Maybe we can't access it directly
            if (!Application.Current.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke((Action)(() => Handle(message)));
                return;
            }

            Application.Current.MainWindow.WindowState = message.WindowState;
        }

        #endregion

        #region IHandleMessage<ListPickerFullModeRequest>

        /// <summary>
        /// Handles a message of a specific type.
        /// </summary>
        /// <param name="message">the message to handle</param>
        public virtual void Handle(ListPickerFullModeRequest message)
        {
            NavigateToPage("/TinyMetroWpfLibrary;component/Controls/Picker/ListPickerFullMode.xaml", message);
        }

        #endregion

        #region IHandleMessage<DatePickerFullModeRequest>

        /// <summary>
        /// Handles a message of a specific type.
        /// </summary>
        /// <param name="message">the message to handle</param>
        public virtual void Handle(DatePickerFullModeRequest message)
        {
            NavigateToPage("/TinyMetroWpfLibrary;component/Controls/Picker/DatePickerFullMode.xaml", message);
        }

        #endregion

        #region IHandleMessage<TimePickerFullModeRequest>

        /// <summary>
        /// Handles a message of a specific type.
        /// </summary>
        /// <param name="message">the message to handle</param>
        public virtual void Handle(TimePickerFullModeRequest message)
        {
            NavigateToPage("/TinyMetroWpfLibrary;component/Controls/Picker/TimePickerFullMode.xaml", message);
        }

        #endregion

        #region IHandleMessage<TimePickerFullModeRequest>

        /// <summary>
        /// Handles a message of a specific type.
        /// </summary>
        /// <param name="message">the message to handle</param>
        public virtual void Handle(TimeSpanPickerFullModeRequest message)
        {
            NavigateToPage("/TinyMetroWpfLibrary;component/Controls/Picker/TimeSpanPickerFullMode.xaml", message);
        }

        #endregion

        #region IHandleMessage<ChangeAnimationModeRequest>

        /// <summary>
        /// This method will change the animation mode
        /// </summary>
        /// <param name="message"></param>
        public virtual void Handle(ChangeAnimationModeRequest message)
        {
            var animationFrame = ContentFrame as AnimationFrame;
            if (animationFrame == null)
                return;

            // Maybe we can't access it directly
            if (!animationFrame.CheckAccess())
            {
                animationFrame.Dispatcher.Invoke((Action)(() => Handle(message)));
                return;
            }

            // Switch the Animation Mode
            AnimationMode oldMode = animationFrame.AnimationMode;
            animationFrame.AnimationMode = message.AnimationMode;
            message.AnimationMode = oldMode;
        }

        #endregion
    }
}