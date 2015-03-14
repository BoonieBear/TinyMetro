// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace TinyMetroWpfLibrary.EventAggregation
{
    /// <summary>
    /// Interface describing an event aggregator.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Subscribes an object to the event aggregator.
        /// </summary>
        /// <param name="subscriber">the subscriber</param>
        void Subscribe(Object subscriber);

        /// <summary>
        /// Unsubscribes an object from the event aggregator (if registered).
        /// </summary>
        /// <param name="subscriber">the subscriber to unregister</param>
        void Unsubscribe(Object subscriber);

        /// <summary>
        /// Publishes a new message to all subsribers of this message type.
        /// </summary>
        /// <typeparam name="TMessage">the type of the message</typeparam>
        /// <param name="message">the message containing data</param>
        /// <returns>the message itself</returns>
        TMessage PublishMessage<TMessage>(TMessage message);
    }
}
