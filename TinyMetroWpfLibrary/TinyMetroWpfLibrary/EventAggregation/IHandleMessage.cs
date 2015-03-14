// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace TinyMetroWpfLibrary.EventAggregation
{
    /// <summary>
    /// Marks the implementer as a message handler of a specific message type. Can 
    /// be implemented multiple times.
    /// </summary>
    /// <typeparam name="TMessage">the type of message to handle</typeparam>
    public interface IHandleMessage<TMessage>
    {
        /// <summary>
        /// Handles a message of a specific type.
        /// </summary>
        /// <param name="message">the message to handle</param>
        void Handle(TMessage message);
    }
}