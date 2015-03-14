using System.Windows;
using System.Windows.Input;
using TinyMetroWpfLibrary.EventAggregation;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.Metro;

namespace TinyMetroWpfLibrary.ViewModel
{
    public class MainWindowViewModelBase : ViewModelBase, 
        IHandleMessage<WindowStateApplicationCommand>
    {
        #region Overrides of ViewModelBase

        /// <summary>
        /// Initializes the ViewModel. 
        /// 
        /// This is used to handle initialization that can't be done in the constructor.
        /// The method should only called once, after the ViewModel has been created.
        /// </summary>
        public override void Initialize()
        {
            // This method registers the WPF Command Close and binds an execute and canExecute method to it.
            RegisterApplicationCommand(ApplicationCommands.Close, ExecuteCloseCommand, CanExecuteCloseCommand, true);

            // This method registers the Minimize Command and binds an execute and canExecute method to it.
            RegisterApplicationCommand(TransparentWindow.MinimizeCommand, ExecuteMinimizeCommand, CanExecuteMinimizeCommand, true);
            RegisterApplicationCommand(TransparentWindow.MaximizeCommand, ExecuteMaximizeCommand, CanExecuteMaximizeCommand, true);
            RegisterApplicationCommand(TransparentWindow.NormalizeCommand, ExecuteNormalizeCommand, CanExecuteNormalizeCommand, true);

            AddPropertyChangedNotification(() => WindowState, TransparentWindow.MaximizeCommand, TransparentWindow.NormalizeCommand);
        }

        /// <summary>
        /// Initializes the Page.
        /// 
        /// This method is used to do some page initialization. 
        /// The calling page can start the new page with some extra data for page initialization.
        /// 
        /// This method is also called on a GoBackNavigationRequest, but without parameter data.
        /// </summary>
        /// <param name="extraData"></param>
        public override void InitializePage(object extraData)
        {
        }

        #endregion

        #region Close Command

        /// <summary>
        /// Determines whether this instance can execute Close command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute Close command; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="sender">The sender.</param>
        public virtual void CanExecuteCloseCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Executes the Close command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void ExecuteCloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            EventAggregator.PublishMessage(new CloseApplicationCommand());
        }

        #endregion

        #region Minimize Command

        /// <summary>
        /// Determines whether this instance can execute Minimize command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute Minimize command; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Event arguments</param>
        public virtual void CanExecuteMinimizeCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        /// <summary>
        /// Executes the Minimize command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Event arguments</param>
        public void ExecuteMinimizeCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new WindowStateApplicationCommand(WindowState.Minimized));
        }

        #endregion

        #region Maximize Command

        /// <summary>
        /// Determines whether this instance can execute Maximize command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute Maximize command; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Event arguments</param>
        public virtual void CanExecuteMaximizeCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = WindowState != WindowState.Maximized;
        }

        /// <summary>
        /// Executes the Maximize command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Event arguments</param>
        public void ExecuteMaximizeCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new WindowStateApplicationCommand(WindowState.Maximized));
        }

        #endregion
        
        #region Normalize Command

        /// <summary>
        /// Determines whether this instance can execute Normalize command.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute Normalize command; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Event arguments</param>
        public virtual void CanExecuteNormalizeCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = WindowState == WindowState.Maximized;
        }

        /// <summary>
        /// Executes the Normalize command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">Event arguments</param>
        public void ExecuteNormalizeCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new WindowStateApplicationCommand(WindowState.Normal));
        }

        #endregion

        #region Property WindowState

        /// <summary>
        /// Gets or sets the Window State
        /// </summary>
        public WindowState WindowState
        {
            get { return GetPropertyValue(() => WindowState); }
            set { SetPropertyValue(() => WindowState, value); }
        }

        #endregion

        #region Implementation of IHandleMessage<WindowStateApplicationCommand>

        /// <summary>
        /// Store the current Window State
        /// </summary>
        /// <param name="message">Window State Message</param>
        public virtual void Handle(WindowStateApplicationCommand message)
        {
            WindowState = message.WindowState;
        }

        #endregion
    }
}
