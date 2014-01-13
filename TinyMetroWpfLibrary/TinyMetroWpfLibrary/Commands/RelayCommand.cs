// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace BoonieBear.TinyMetro.WPF.Commands
{
    public class RelayCommand : ICommand
    {
        private static readonly ConstructorInfo ExecutedRoutedEventArgsConstructor =
            typeof (ExecutedRoutedEventArgs).GetConstructors(
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).First();

        private static readonly ConstructorInfo CanExecuteRoutedEventArgsConstructor = 
            typeof(CanExecuteRoutedEventArgs).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).First();

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<Object, ExecutedRoutedEventArgs> execute, Action<Object, CanExecuteRoutedEventArgs> canExecute)
        {
            ExecuteAction = execute;
            CanExecuteAction = canExecute;

            if (execute != null)
                OwnsExecute = true;

            // get hold of dispatcher
            if (Application.Current != null)
                CurrentDispatcher = Application.Current.Dispatcher;
        }

        /// <summary>
        /// Gets or sets the execute action.
        /// </summary>
        /// <value>The execute action.</value>
        protected Action<Object, ExecutedRoutedEventArgs> ExecuteAction { get; private set; }

        /// <summary>
        /// Gets or sets the can execute predicate.
        /// </summary>
        /// <value>The can execute predicate.</value>
        protected Action<Object, CanExecuteRoutedEventArgs> CanExecuteAction { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [owns execute].
        /// </summary>
        /// <value><c>true</c> if [owns execute]; otherwise, <c>false</c>.</value>
        protected bool OwnsExecute { get; private set; }

        /// <summary>
        /// Gets or sets the current dispatcher.
        /// </summary>
        /// <value>The current dispatcher.</value>
        protected Dispatcher CurrentDispatcher { get; private set; }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            if (ExecuteAction == null) 
                return;

            var args = (ExecutedRoutedEventArgs) ExecutedRoutedEventArgsConstructor.Invoke(new object[]{this, null});
            ExecuteAction(parameter, args);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public bool CanExecute(object parameter)
        {
            if (CanExecuteAction == null) 
                return false;

            var args = (CanExecuteRoutedEventArgs) CanExecuteRoutedEventArgsConstructor.Invoke(new[] {this, null});
                
            CanExecuteAction(parameter, args);
            return args.CanExecute;
        }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the can execute changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler == null) 
                return;

            if (CurrentDispatcher != null && !CurrentDispatcher.CheckAccess())
            {
                CurrentDispatcher.BeginInvoke((Action)RaiseCanExecuteChanged);
            }
            else
                handler(this, EventArgs.Empty);      // already on the UI thread
        }
    }
}
