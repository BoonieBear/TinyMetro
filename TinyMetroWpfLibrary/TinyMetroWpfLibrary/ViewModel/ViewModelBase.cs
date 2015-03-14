// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using TinyMetroWpfLibrary.Commands;
using TinyMetroWpfLibrary.Controller;
using TinyMetroWpfLibrary.EventAggregation;

namespace TinyMetroWpfLibrary.ViewModel
{
    /// <summary>
    /// The ViewModelBase is used as base for all view models
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        #region Fields

        /// <summary>
        /// Contains the values of all properties of the view model.
        /// </summary>
        private readonly IDictionary<string, object> propertyValues;

        /// <summary>
        /// Contains the dependent notifications that shall be send if a property changes.
        /// </summary>
        private readonly IDictionary<string, IList<string>> dependentNotifications;

        /// <summary>
        /// Contains the dependent commands that shall be invalidated when a property changes.
        /// </summary>
        private readonly IDictionary<string, IList<ICommand>> dependentCommandNotifications;

        /// <summary>
        /// loading indicator lock object
        /// </summary>
        private static readonly object IsLoadingLock = new object();

        /// <summary>
        /// counter for the loading indicator
        /// </summary>
        private int isLoadingCounter;

        /// <summary>
        /// contains the registered commands
        /// </summary>
        private readonly CommandBindingCollection registeredCommands = new CommandBindingCollection();

        #endregion

        #region Constructor / destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        /// <param name="eventAggregator">Used Event aggregator, or null if the default Kernel Event Aggregator shall be used</param>
        protected ViewModelBase(IEventAggregator eventAggregator = null)
        {
            // initialize the maps
            propertyValues = new Dictionary<string, object>();
            dependentNotifications = new Dictionary<string, IList<string>>();
            dependentCommandNotifications = new Dictionary<string, IList<ICommand>>();

            // initialize loading indicator
            isLoadingCounter = 0;

            if (!IsDesignMode)
            {
                // subscribe to the event aggregator messages
                this.eventAggregator = eventAggregator ?? Kernel.Instance.EventAggregator;
                this.eventAggregator.Subscribe(this);
            }

            // default dependent notifications
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            AddPropertyChangedNotification(() => IsLoading, () => IsReady, () => ShowDefective);
            AddPropertyChangedNotification(() => IsDefective, () => ShowDefective);
            AddPropertyChangedNotification(() => Error, () => IsDefective);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        /// <summary>
        /// Finalizes an instance of the ViewModelBase class
        /// </summary>
        ~ViewModelBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Initializes the static members of the ViewModel Base
        /// </summary>
        static ViewModelBase()
        {
            try
            {
                isDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
            }
            catch (Exception)
            {
                isDesignMode = true;
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Initializes the page.
        /// This method will be called every time the user navigates to the page
        /// </summary>
        /// <param name="extraData">The extra Data, if there's any. Otherwise NULL</param>
        public abstract void InitializePage(object extraData);

        #endregion

        #region Command Bindings

        /// <summary>
        /// Gets the command bindings.
        /// </summary>
        /// <value>The command bindings.</value>
        public CommandBindingCollection RegisteredCommands
        {
            get
            {
                return registeredCommands;
            }
        }

        /// <summary>
        /// Creates a command which is automatically disabled when the IsLoading indicator is set to true and
        /// enabled or disabled automatically based on the required permissions
        /// </summary>
        public virtual void RegisterApplicationCommand(
            ICommand commandToRegister, 
            Action<Object,ExecutedRoutedEventArgs> execute, 
            Action<Object,CanExecuteRoutedEventArgs> canExecute, 
            bool disableOnLoading)
        {
            var command = new CommandBinding(commandToRegister,
                (sender, parameter) => ExecuteRegisteredCommand(execute, sender, parameter),
                (sender, parameter) => CanExecuteRegisteredCommand(canExecute, sender, parameter, disableOnLoading));

            CommandManager.RegisterClassCommandBinding(GetType(), command);

            RegisteredCommands.Add(command);
        }

        /// <summary>
        /// Creates a command which is automatically disabled when the <see cref="IsLoading"/> indicator is set to true and enabled or disabled automatically based on the required permissions
        /// </summary>
        /// <param name="execute">the command action</param>
        /// <param name="canExecute">the predicate to check, if the command can be executed</param>
        /// <param name="disableOnLoading">a value indicating whether the command should be disabled when the <see cref="IsLoading"/> indicator is set to true</param>
        public virtual ICommand RegisterCommand(
            Action<Object,ExecutedRoutedEventArgs> execute, 
            Action<Object,CanExecuteRoutedEventArgs> canExecute, 
            bool disableOnLoading)
        {
            var commandToRegister = new RelayCommand(
                (sender, parameter) => ExecuteRegisteredCommand(execute, sender, parameter),
                (sender, parameter) => CanExecuteRegisteredCommand(canExecute, sender, parameter, disableOnLoading));

            if (disableOnLoading)
                AddPropertyChangedNotification(()=>IsLoading, commandToRegister);

            return commandToRegister;
        }

        /// <summary>
        /// Executes a registered command.
        /// </summary>
        private static void ExecuteRegisteredCommand(Action<Object, ExecutedRoutedEventArgs> execute, object sender, ExecutedRoutedEventArgs parameter)
        {
            execute(sender, parameter);
        }

        /// <summary>
        /// Returns a value indicating, whether a registered command can be executed, which is the case if the <see cref="IsLoading"/> indicator is not set to true
        /// and the user has the required permissions and the command's predicate is fulfilled
        /// </summary>
        private void CanExecuteRegisteredCommand(Action<Object, CanExecuteRoutedEventArgs> canExecute, object sender, CanExecuteRoutedEventArgs parameter, bool disableOnLoading)
        {
            canExecute(sender, parameter);
            parameter.CanExecute = (!disableOnLoading || !IsLoading) && parameter.CanExecute;
        }

        #endregion

        #region PropertyChanged event

        /// <summary>
        /// Raised when a property on this object gets a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// This field is used to identify if the code is called in DesignMode or in real life ;)
        /// </summary>
        private static readonly bool isDesignMode;

        /// <summary>
        /// Used Event Aggregator
        /// </summary>
        private readonly IEventAggregator eventAggregator;

        /// <summary>
        /// Gets the Property Changed Handler.
        /// </summary>
        /// <value>The property.</value>
        protected PropertyChangedEventHandler NotifyHandler
        {
            get { return PropertyChanged; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the event aggregator of the application.
        /// </summary>
        protected virtual IEventAggregator EventAggregator
        {
            get { return eventAggregator; }
        }

        /// <summary>
        /// Gets the dictionary of the property values, which maps the name of a property to its current value. 
        /// </summary>
        protected IDictionary<string, Object> PropertyValues
        {
            get { return propertyValues; }
        }

        /// <summary>
        /// Gets the dictionary, which maps the name of a base property to a list of dependent property names
        /// </summary>
        protected IDictionary<string, IList<string>> DependentNotifications
        {
            get { return dependentNotifications; }
        }

        /// <summary>
        /// Gets the dictionary, which maps the name of a base property to a list of dependent property names
        /// </summary>
        protected IDictionary<string, IList<ICommand>> DependentCommandNotifications
        {
            get { return dependentCommandNotifications; }
        }

        /// <summary>
        /// Gets or sets the error message indicating what is wrong with this object.
        /// </summary>
        public string Error
        {
            get { return GetPropertyValue(() => Error); }
            set { SetPropertyValue(() => Error, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the ViewModel is defect
        /// </summary>
        public bool IsDefective
        {
            get { return !string.IsNullOrEmpty(Error); }
        }

        /// <summary>
        /// Gets a value indicating whether the error message shall be shown
        /// </summary>
        public bool ShowDefective
        {
            get { return IsDefective && !IsLoading; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view model is currently loading data and 
        /// therefore other actions should be prevented.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return GetPropertyValue(() => IsLoading);
            }

            set
            {
                Monitor.Enter(IsLoadingLock);

                try
                {
                    if (value)
                    {
                        ++isLoadingCounter;
                    }
                    else
                    {
                        --isLoadingCounter;
                        if (isLoadingCounter < 0)
                        {
                            isLoadingCounter = 0;
                        }
                    }

                    var isLoading = isLoadingCounter > 0;
                    SetPropertyValue(() => IsLoading, isLoading);
                }
                finally
                {
                    Monitor.Exit(IsLoadingLock);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the view model is ready for user interaction
        /// </summary>
        public bool IsReady
        {
            get { return !IsLoading; }
        }

        /// <summary>
        /// Gets a value indicating whether the view model is in design mode
        /// </summary>
        public static bool IsDesignMode
        {
            get
            {
                return isDesignMode;
            }
        }

        #endregion

        #region Property handling

        /// <summary>Gets the current value of a property.</summary>
        /// <typeparam name="T">the type of the property</typeparam>
        protected T GetPropertyValue<T>(Expression<Func<T>> propertyAccessor)
        {
            return GetPropertyValue<T>(GetPropertyName(propertyAccessor));
        }

        /// <summary>Gets the current value of a property.</summary>
        /// <typeparam name="T">the type of the property</typeparam>
        protected T GetPropertyValue<T>(string propertyName)
        {
            object result;
            if (PropertyValues.TryGetValue(propertyName, out result))
                return (T) result;

            return default(T);
        }

        /// <summary>
        /// Sets the value of a property. If the value is different the current value of the property, <see cref="PropertyChanged"/> event is raised.
        /// </summary>
        /// <typeparam name="T">the type of the property</typeparam>
        /// <returns>True if the property value has been updated, otherwise false</returns>
        protected bool SetPropertyValue<T>(Expression<Func<T>> propertyAccessor, T value) 
        {
            bool valueChanged;

            string propertyName = GetPropertyName(propertyAccessor);
            if (PropertyValues.ContainsKey(propertyName))
            {
                var previousValue = GetPropertyValue<T>(propertyName);

                if (typeof(T).IsValueType)
                {
                    valueChanged = !previousValue.Equals(value);
                }
                else
                {
// ReSharper disable CompareNonConstrainedGenericWithNull
                    valueChanged = previousValue != null ? !previousValue.Equals(value) : value != null;
// ReSharper restore CompareNonConstrainedGenericWithNull
                }

                PropertyValues[propertyName] = value;
                if (valueChanged)
                    OnPropertyChanged(propertyName);
            }
            else
            {
                valueChanged = true;
                PropertyValues.Add(propertyName, value);

                OnPropertyChanged(propertyName);
            }

            return valueChanged;
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for a property and all its dependent properties (that have been registered by the AddPropertyChangedNotification method.
        /// </summary>
        /// <param name="propertyName">the name of the property</param>
        protected void OnPropertyChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName, true, new Dictionary<string, Object>(), new Dictionary<ICommand, Object>());
        }

        /// <summary>
        /// Raises the PropertyChanged event for a property and - if requested - all its dependent properties (that have been registered by the AddPropertyChangedNotification method.
        /// </summary>
        /// <typeparam name="T">the type of the property</typeparam>
        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyAccessor, bool notifyDependentProperties = true)
        {
            var propertyName = GetPropertyName(propertyAccessor);
            RaisePropertyChanged(propertyName, notifyDependentProperties, new Dictionary<string, Object>(), new Dictionary<ICommand, Object>());
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        private void RaisePropertyChanged(string propertyName, bool notifyDependentProperties, 
            IDictionary<string, object> processedProperties, 
            IDictionary<ICommand, object> processedCommands)
        {
            if (processedProperties.ContainsKey(propertyName)) 
                return;

            var eventArgs = new PropertyChangedEventArgs(propertyName);

            if (PropertyChanged != null)
                Dispatch(() => PropertyChanged(this, eventArgs));

// ReSharper disable AssignNullToNotNullAttribute
            processedProperties.Add(propertyName, null);
// ReSharper restore AssignNullToNotNullAttribute

            if (!notifyDependentProperties) 
                return;

            IList<string> dependentPropertyNotifications;
            if (DependentNotifications.TryGetValue(propertyName, out dependentPropertyNotifications))
            {
                foreach (var dependentProperty in dependentPropertyNotifications)
                    RaisePropertyChanged(dependentProperty, true, processedProperties, processedCommands);
            }

            IList<ICommand> dependendCommandNotifications;
            if (DependentCommandNotifications.TryGetValue(propertyName, out dependendCommandNotifications))
            {
                foreach (var dependentCommand in dependendCommandNotifications)
                {
                    if (processedCommands.ContainsKey(dependentCommand)) 
                        continue;

                    var relayCommand = dependentCommand as RelayCommand;
                    if (relayCommand != null)
                        relayCommand.RaiseCanExecuteChanged();

// ReSharper disable AssignNullToNotNullAttribute
                    processedCommands.Add(dependentCommand, null);
// ReSharper restore AssignNullToNotNullAttribute
                }
            }
        }

        /// <summary>
        /// Adds a new entry to the list of dependent property notifications.
        /// </summary>
        /// <typeparam name="TSource">the type of the source property</typeparam>
        /// <typeparam name="TDepend">the type of the dependent property</typeparam>
        public virtual void AddPropertyChangedNotification<TSource, TDepend>(
            Expression<Func<TSource>> sourcePropertyAccessor, 
            params Expression<Func<TDepend>>[] dependentPropertyAccessors)
        {
            var basePropertyName = GetPropertyName(sourcePropertyAccessor);

            foreach (var dependentPropertyName in dependentPropertyAccessors
                .Select(GetPropertyName)
                .Where(dependentPropertyName => basePropertyName != dependentPropertyName))
            {
                IList<string> notifications;

                if (!DependentNotifications.TryGetValue(basePropertyName, out notifications))
                {
                    notifications = new List<string>();
                    DependentNotifications.Add(basePropertyName, notifications);
                }

                notifications.Add(dependentPropertyName);
            }
        }

        /// <summary>
        /// Adds a new entry to the list of dependent command notifications.
        /// </summary>
        /// <typeparam name="TSource">the type of the source property</typeparam>
        public virtual void AddPropertyChangedNotification<TSource>(Expression<Func<TSource>> sourcePropertyAccessor, params ICommand[] dependendCommands)
        {
            // get the name of the source property
            var basePropertyName = GetPropertyName(sourcePropertyAccessor);
            AddPropertyChangedNotification(basePropertyName, dependendCommands);
        }

        /// <summary>
        /// Adds the dependent notification.
        /// </summary>
        /// <param name="basePropertyName">Name of the base property.</param>
        /// <param name="dependendCommands">The dependend commands.</param>
        public virtual void AddPropertyChangedNotification(string basePropertyName, params ICommand[] dependendCommands)
        {
            IList<ICommand> notifications;

            if (DependentCommandNotifications.ContainsKey(basePropertyName))
            {
                notifications = DependentCommandNotifications[basePropertyName];
            }
            else
            {
                notifications = new List<ICommand>();
                DependentCommandNotifications.Add(basePropertyName, notifications);
            }

            foreach (var notification in dependendCommands.Except(notifications))
                notifications.Add(notification);
        }

        #endregion

        #region Threading

        /// <summary>
        /// Performs a specific action on the UI thread.
        /// </summary>
        /// <param name="action">the action to execute</param>
        public virtual void Dispatch(Action action)
        {
            var application = Application.Current;

            if (application == null)
                action();
            else
                // check, if we have access to the UI thread
                if (application.Dispatcher.CheckAccess())
                    action(); // execute directly
                else
                    application.Dispatcher.BeginInvoke(action); // place the action on the Dispatcher of the UI thread
        }

        /// <summary>
        /// Performs an delayed action on the UI thread
        /// </summary>
        /// <param name="action">the action to execute</param>
        public virtual void DispatchDelayed(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action); 
        }

        /// <summary>
        /// Executes an action asynchronously.
        /// </summary>
        /// <param name="action">the action to execute</param>
        /// <param name="delay">a delay in milliseconds to wait before the action is executed (pass a value
        /// greate or equal to zero to execute the action without any delay)</param>
        public virtual void ExecuteAsync(Action action, int delay)
        {
            ExecuteAsync<object>(param => action(), null, delay);
        }

        /// <summary>
        /// Executes an action, that takes one parameter, asynchronously.
        /// </summary>
        /// <typeparam name="T">the type of the parameter for the action</typeparam>
        /// <param name="action">the action to execute</param>
        /// <param name="parameter">the parameter to pass to the action</param>
        /// <param name="delay">a delay in milliseconds to wait before the action is executed (pass a value
        /// greate or equal to zero to execute the action without any delay)</param>
        public virtual void ExecuteAsync<T>(Action<T> action, T parameter, int delay)
        {
            // start new thread from the thread pool and pass it the action and the parameter
            ThreadPool.QueueUserWorkItem(
                threadParameter =>
                {
                    var threadParameters = (Object[])threadParameter;

                    // check, if the action execution should be delayed
                    if (delay > 0)
                        Thread.Sleep(delay);

                    // execute the action
                    ((Action<T>)threadParameters[0])((T)threadParameters[1]);
                },
                new Object[] { action, parameter });
        }

        #endregion

        #region Disposing

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Child classes can override this method to perform
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        public virtual void Dispose(bool disposing)
        {
            if (IsDesignMode)
                return;

            if (IsDisposed)
                throw new ObjectDisposedException("ViewModel has already been disposed");

            if (disposing)
            {
                // unsubscribe from the event aggregator
                Dispatch(() => EventAggregator.Unsubscribe(this));
            }

            IsDisposed = true;
        }

        /// <summary>
        /// True, if the ViewModel has been disposed
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets or sets the faulted command that can be re-run
        /// </summary>
        public ICommand FaultedCommand
        {
            get { return GetPropertyValue(() => FaultedCommand); }
            set { SetPropertyValue(() => FaultedCommand, value); }
        }

        #endregion

        #region Property Helper

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public static string GetPropertyName<T>(Expression<Func<T>> propertyAccessor)
        {
            return ((MemberExpression)propertyAccessor.Body).Member.Name;
        }

        #endregion
    }
}