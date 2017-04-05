using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Characters.FirstPerson;

namespace Phantom.Utility.MessageBus
{
    public interface IMessageBus
    {
        void PublishEvent<TEventType>(TEventType evt);
        void Subscribe(object subscriber);
    }

    public interface ISubscriber<TEventType>
    {
        void OnEvent(TEventType evt);
    }

    public class MessageBus : IMessageBus
    {
        private readonly Dictionary<Type, List<WeakReference>> _subscribers =
        new Dictionary<Type, List<WeakReference>>();

        private readonly object _lockObj = new object();

        public void PublishEvent<TEventType>(TEventType evt)
        {
            var subscriberType = typeof(ISubscriber<>).MakeGenericType(typeof(TEventType));

            var subscribers = GetSubscriberList(subscriberType);

            List<WeakReference> subsToRemove = new List<WeakReference>();
            foreach (var weakSubscriber in subscribers)
            {
                
                if (weakSubscriber.IsAlive)
                {
                    var subscriber = (ISubscriber<TEventType>)weakSubscriber.Target;
                    InvokeSubscriberEvent(evt, subscriber);
                }
                else
                {
                    subsToRemove.Add(weakSubscriber);
                }
            }

            // Remove any dead subscribers.
            if (subsToRemove.Count > 0)
            {
                lock (_lockObj)
                {
                    foreach (var remove in subsToRemove)
                    {
                        subscribers.Remove(remove);
                    }
                }
            }
        }

        //not working yet?
        public void UnSubscribe(object subscriber) {

            lock (_lockObj)
            {

                var subscriberTypes =
                subscriber.GetType()
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISubscriber<>));

                WeakReference weakRef = new WeakReference(subscriber);

                foreach (var subscriberType in subscriberTypes)
                {
                    List<WeakReference> subscribers = GetSubscriberList(subscriberType);
                    subscribers.Remove(weakRef);
                }
            }
        }

        public void Subscribe(object subscriber)
        {
            lock (_lockObj)
            {
                var subscriberTypes =
                subscriber.GetType()
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISubscriber<>));

                WeakReference weakRef = new WeakReference(subscriber);

                foreach (var subscriberType in subscriberTypes)
                {
                    List<WeakReference> subscribers = GetSubscriberList(subscriberType);
                    subscribers.Add(weakRef);
                }
            }
        }

        private void InvokeSubscriberEvent<TEventType>(TEventType evt, ISubscriber<TEventType> subscriber)
        {
            subscriber.OnEvent(evt);
        }

        private List<WeakReference> GetSubscriberList(Type subscriberType)
        {
            List<WeakReference> subscribersList = null;

            lock (_lockObj)
            {
                bool found = _subscribers.TryGetValue(subscriberType, out subscribersList);

                if (!found)
                {
                    // Create the list.
                    subscribersList = new List<WeakReference>();
                    _subscribers.Add(subscriberType, subscribersList);
                }
            }
            return subscribersList;
        }
    }

    public class MainBus
    {
        private static MessageBus _instance;

        public static MessageBus Instance
        {
            get { return _instance ?? (_instance = new MessageBus()); }
        }
    }

    //Event Classes
    public class SpottedEvent {
        public Transform targetTransform;
        public SpottedEvent(Transform target) {
            targetTransform = target;
        }
    }

    public class LostSightEvent
    {
    }

    public class TryingToDragAlive {
        public GameObject target;
        public TryingToDragAlive(GameObject target) {
            this.target = target;
        }
    }

    public class ChokeEnemy
    {
        public GameObject target;
        public ChokeEnemy(GameObject target) {
            this.target = target;
        }
    }

    public class KillEnemy {
        public GameObject target;
        public KillEnemy(GameObject target)
        {
            this.target = target;
        }
    }
}
