// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;

namespace TinyMetroWpfLibrary.EventAggregation
{
    /// <summary>
    /// Publishes messages to all objects subscribed for the message type.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        #region - Members -----------------------------------------------------------------

        /// <summary>
        /// the list of subscribers to this event aggregator
        /// </summary>
        private readonly List<WeakReference> subscribers;

        /// <summary>
        /// Synchronization object for the subscribers
        /// </summary>
        private readonly Object subscribersLock;

        #endregion

        #region - Constructors ------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAggregator"/> class.
        /// </summary>
        public EventAggregator()
        {
            subscribers = new List<WeakReference>();
            subscribersLock = new Object();
        }

        #endregion

        #region - Properties --------------------------------------------------------------

        /// <summary>
        /// Gets all current subscribers for all messages.
        /// </summary>
        /// <value>the registered subscribers</value>
        private List<WeakReference> Subscribers
        {
            get { return subscribers; }
        }

        #endregion

        #region - IEventAggregator implementation -----------------------------------------

        /// <summary>
        /// Subscribes an object to the event aggregator.
        /// </summary>
        /// <param name="subscriber">the subscriber</param>
        public virtual void Subscribe(Object subscriber)
        {
            if (subscriber != null)
            {
                Monitor.Enter(subscribersLock);
                try
                {
                    Subscribers.Add(new WeakReference(subscriber));
                }
                finally
                {
                    Monitor.Exit(subscribersLock);
                }
            }
        }

        /// <summary>
        /// Unsubscribes an object from the event aggregator (if registered).
        /// </summary>
        /// <param name="subscriber">the subscriber to unregister</param>
        public virtual void Unsubscribe(Object subscriber)
        {
            Monitor.Enter(subscribersLock);
            try
            {
                // the index of the subscriber
                int indexOf = -1;

                // iterate through all the subscribers
                for (int i = 0; (i < Subscribers.Count) && (indexOf == -1); ++i)
                {
                    {
                        WeakReference subscribedObject = Subscribers[i];

                        // check, if it's the subscriber we're looking for
                        if (subscribedObject.IsAlive && subscribedObject.Target.Equals(subscriber))
                        {
                            // remember index of subscriber
                            indexOf = i;
                        }
                    }

                    // check, if we found the subscriber
                    if (indexOf >= 0)
                    {
                        // remove the subscriber
                        Subscribers.RemoveAt(indexOf);
                    }
                }
            }
            finally
            {
                Monitor.Exit(subscribersLock);
            }
        }

        /// <summary>
        /// Publishes a new message to all subsribers of this message type.
        /// </summary>
        /// <typeparam name="TMessage">the type of the message</typeparam>
        /// <param name="message">the message containing data</param>
        /// <returns>the message itself</returns>
        public virtual TMessage PublishMessage<TMessage>(TMessage message)
        {
            // get all subscribers for the message
            IEnumerable<IHandleMessage<TMessage>> allSubscribersFor = GetAllSubscribersFor<TMessage>();

            // let all subscribers of the message handle it
            foreach (IHandleMessage<TMessage> subscriber in allSubscribersFor)
            {
                subscriber.Handle(message);
            }

            return message;
        }

        #endregion

        #region - Helper methods ----------------------------------------------------------

        /// <summary>
        /// Gets all subscribers for a specific message type.
        /// </summary>
        /// <typeparam name="TMessage">the message type to search registered implementers for</typeparam>
        /// <returns>the list of subscribers</returns>
        private IEnumerable<IHandleMessage<TMessage>> GetAllSubscribersFor<TMessage>()
        {
            var result = new List<IHandleMessage<TMessage>>();

            Monitor.Enter(subscribersLock);
            try
            {
                // the list of indexes where the subscriber has been garbage collected and therefore can be removed
                var indexesToRemove = new List<int>();

                // iterate through the list of subscribers
                for (var i = 0; i < Subscribers.Count; ++i)
                {
                    var subscribedObject = Subscribers[i];

                    // check, if it's a subscriber for the requested message type
                    if (subscribedObject.IsAlive)
                    {
                        var subscriber = subscribedObject.Target as IHandleMessage<TMessage>;
                        if (subscriber != null)
                        {
                            result.Add(subscriber);
                        }
                    }
                    else
                    {
                        indexesToRemove.Add(i);
                    }
                }

                // remove all the subscribers that have been garbage collected
                for (int i = indexesToRemove.Count - 1; i >= 0; --i)
                {
                    Subscribers.RemoveAt(indexesToRemove[i]);
                }
            }
            finally
            {
                Monitor.Exit(subscribersLock);
            }

            return result;
        }

        #endregion
    }
}
