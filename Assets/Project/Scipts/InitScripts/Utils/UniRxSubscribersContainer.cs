using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Project
{
    public class UniRxSubscribersContainer
    {
        private readonly Dictionary<object, List<IDisposable>> _subscribeMap = null;

        public UniRxSubscribersContainer()
        {
            _subscribeMap = new Dictionary<object, List<IDisposable>>();
        }

        public void Subscribe<T>(IObservable<T> reactProp, Action<T> callback, object subscriptionKey = null)
        {
            FixSubscriptionKey(ref subscriptionKey);

            if (!_subscribeMap.TryGetValue(subscriptionKey, out var subscribers))
            {
                subscribers = new List<IDisposable>();
                _subscribeMap[subscriptionKey] = subscribers;
            }

            var res = reactProp.Subscribe(callback);

            subscribers.Add(res);
        }

        public void FreeAllSubscribers()
        {
            foreach (var kvp in _subscribeMap)
            {
                foreach (var subscriber in kvp.Value)
                {
                    subscriber.Dispose();
                }

                kvp.Value.Clear();
            }

            _subscribeMap.Clear();
        }

        public void FreeSubscribers(object subscriptionKey = null)
        {
            FixSubscriptionKey(ref subscriptionKey);

            if (!_subscribeMap.TryGetValue(subscriptionKey, out var subscribers))
            {
                return;
            }

            foreach (var subscriber in subscribers)
            {
                subscriber.Dispose();
            }

            subscribers.Clear();
        }

        public bool IsEmpty()
        {
            if (_subscribeMap.Count == 0)
            {
                return true;
            }

            if (_subscribeMap.All(kvp => kvp.Value.Count == 0))
            {
                return true;
            }

            return false;
        }

        private void FixSubscriptionKey(ref object subscriptionKey)
        {
            if (subscriptionKey == null)
            {
                subscriptionKey = this;
            }
        }
    }
}