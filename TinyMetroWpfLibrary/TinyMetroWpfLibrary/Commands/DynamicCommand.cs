// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Input;

namespace BoonieBear.TinyMetro.WPF.Commands
{
    /// <summary>
    /// A Dynamic Command is an lightweight alternative to the RelayCommand.
    /// It's only used for viewmodel in-code commands
    /// </summary>
    public class DynamicCommand : ICommand
    {
        private readonly Action<object> executeAction;
        private readonly Func<bool> canExecuteAction;

        /// <summary>
        /// Initializes a new instance of the Dynamic Command with one execute Action
        /// </summary>
        /// <param name="executeAction">Action that will be executed</param>
        public DynamicCommand(Action<object> executeAction)
        {
            this.executeAction = executeAction;
            this.canExecuteAction = () => true;
        }

        /// <summary>
        /// Initializes a new instance of the Dynamic Command with an execute Action and a proper CanExecuteAction Func
        /// </summary>
        /// <param name="canExecuteAction">Func for determinating if the action can be executed</param>
        /// <param name="executeAction">Action that will be executed</param>
        public DynamicCommand(Func<bool> canExecuteAction, Action<object> executeAction)
        {
            this.executeAction = executeAction;
            this.canExecuteAction = canExecuteAction;
        }

        #region Implementation of ICommand

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            executeAction(parameter);
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
            return canExecuteAction();
        }

        /// <summary>
        /// Defines the public event handler that will be fired, if the CanExecute Method changes, which is never the case with a Dynamic Command
        /// </summary>
        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
