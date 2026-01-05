#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Common
{

    public abstract class AbstractEvent
    {
    }

    public class EventsManager : IDisposable
    {
        private readonly Dictionary<object, List<(Type, Action<AbstractEvent>)>> _eventHandlersByOwner = new();
        private readonly Dictionary<Type, List<Action<AbstractEvent>>> _eventHandlersByType = new();
        private readonly List<EventData> _eventsList = new();

        public void Dispose()
        {
            ReleaseAll();
        }
        
        public void Update(float deltaTime)
        {
            var i = 0;
            while (i < _eventsList.Count)
            {
                var eventData = _eventsList[i];
                eventData.Update(deltaTime);

                if (!eventData.IsReady())
                {
                    ++i;
                    continue;
                }

                var gameEvent = eventData.GetEvent();
                var eventType = gameEvent.GetType();

                if (_eventHandlersByType.TryGetValue(eventType, out var handlerList))
                {
                    foreach (var eventHandler in handlerList)
                    {
                        try
                        {
                            eventHandler.Invoke(gameEvent);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogException(ex);
                        }
                    }
                }

                _eventsList.RemoveAt(i);
            }
        }

        public void Subscribe<T>(object owner, Action<T> callback) where T : AbstractEvent
        {
            var eventType = typeof(T);

            if (!_eventHandlersByType.TryGetValue(eventType, out var handlerList))
            {
                handlerList = new List<Action<AbstractEvent>>();
                _eventHandlersByType.Add(eventType, handlerList);
            }

            var wrappedHandler = new Action<AbstractEvent>(gameEvent => callback((T)gameEvent));
            handlerList.Add(wrappedHandler);

            if (!_eventHandlersByOwner.TryGetValue(owner, out var handlerByOwnerList))
            {
                handlerByOwnerList = new List<(Type, Action<AbstractEvent>)>();
                _eventHandlersByOwner.Add(owner, handlerByOwnerList);
            }

            handlerByOwnerList.Add((eventType, wrappedHandler));
        }

        public void ReleaseByOwner(object owner)
        {
            if (!_eventHandlersByOwner.TryGetValue(owner, out var ownerHandlerList))
            {
                return;
            }

            foreach (var handlerByType in ownerHandlerList)
            {
                if (!_eventHandlersByType.TryGetValue(handlerByType.Item1, out var handlerList))
                {
                    continue;
                }

                handlerList.Remove(handlerByType.Item2);
            }

            _eventHandlersByOwner.Remove(owner);
        }

        public void ReleaseAll()
        {
            _eventHandlersByType.Clear();
            _eventHandlersByOwner.Clear();
        }

        public void PublishEvent(AbstractEvent eventArgs, float delay = 0)
        {
            _eventsList.Add(new EventData(eventArgs, delay));
        }

        private class EventData
        {
            private readonly AbstractEvent _event;
            private float _delay;

            public EventData(AbstractEvent eventArgs, float delay)
            {
                _event = eventArgs;
                _delay = delay;
            }

            public void Update(float deltaTime)
            {
                _delay -= deltaTime;
            }

            public bool IsReady() => _delay <= 0;

            public AbstractEvent GetEvent() => _event;
        }
    }
}