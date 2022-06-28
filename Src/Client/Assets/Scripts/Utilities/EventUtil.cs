using System;
using System.Collections.Generic;

namespace Utilities
{
    public abstract class GameEvent
    {
    }
    
    public static class EventUtil
    {
        static readonly Dictionary<Type, Action<GameEvent>> events = new Dictionary<Type, Action<GameEvent>>();
        static readonly Dictionary<Delegate, Action<GameEvent>> eventLookups =
            new Dictionary<Delegate, Action<GameEvent>>();

        public static void AddListener<T>(Action<T> evt) where T : GameEvent
        {
            if (!eventLookups.ContainsKey(evt))
            {
                Action<GameEvent> newAction = (e) => evt((T) e);
                eventLookups[evt] = newAction;

                if (events.TryGetValue(typeof(T), out Action<GameEvent> internalAction))
                    events[typeof(T)] = internalAction += newAction;
                else
                    events[typeof(T)] = newAction;
            }
        }

        public static void RemoveListener<T>(Action<T> evt) where T : GameEvent
        {
            if (eventLookups.TryGetValue(evt, out var action))
            {
                if (events.TryGetValue(typeof(T), out var tempAction))
                {
                    tempAction -= action;
                    if (tempAction == null)
                        events.Remove(typeof(T));
                    else
                        events[typeof(T)] = tempAction;
                }

                eventLookups.Remove(evt);
            }
        }

        public static void Broadcast(GameEvent evt)
        {
            if (events.TryGetValue(evt.GetType(), out var action))
                action.Invoke(evt);
        }

        public static void Clear()
        {
            events.Clear();
            eventLookups.Clear();
        }
    }
}
