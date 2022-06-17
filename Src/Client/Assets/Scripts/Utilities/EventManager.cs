using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Utilities
{
    /// <summary>
    /// The events base class. the classes derived from it is in Events.cs
    /// </summary>
    public class GameEvent
    {
    
    }
    /// <summary>
    /// Just for service global event.
    /// </summary>
    public static class EventManager
    {
        static readonly Dictionary<Type, Action<GameEvent>> events = new Dictionary<Type, Action<GameEvent>>();
        static readonly Dictionary<Delegate, Action<GameEvent>> eventLookups = new Dictionary<Delegate, Action<GameEvent>>();

        public static void AddListener<T>(Action<T> evt) where T : GameEvent
        {
            if (!eventLookups.ContainsKey(evt))
            {
                Action<GameEvent> newAction = (e) => evt((T)e);
                eventLookups[evt] = newAction;

                if (events.TryGetValue((typeof(T)), out Action<GameEvent> internalAction))
                {
                    events[typeof(T)] = internalAction += newAction;
                    Debug.LogFormat("添加监听器{0}",typeof(T));
             

                }
                else
                {
                    events[typeof(T)] = newAction;
                    Debug.LogFormat("添加第一个监听器{0}",typeof(T));
                }
            }
        }

        public static void RemoveListener<T>(Action<T> evt) where T : GameEvent
        {
            if (eventLookups.TryGetValue(evt, out var action))
            {
                if (events.TryGetValue(typeof(T), out var internalAction))
                {
                    internalAction -= action;
                    if (internalAction == null)
                    {
                        events.Remove(typeof(T));
                        Debug.LogFormat("删除监听器{0}",typeof(T));
                    }
                    else
                    {
                        events[typeof(T)] = internalAction;
                        Debug.LogFormat("删除最后一个监听器{0}",typeof(T));
                    }

                    eventLookups.Remove(evt);
                }
            }
        }

        public static void Broadcast(GameEvent evt)
        {
            if (events.TryGetValue(evt.GetType(), out var action))
            {
                action.Invoke(evt);
                Debug.LogFormat("成功广播");
            }
        }

        public static void Clear()
        {
            events.Clear();
            eventLookups.Clear();
        }
    }
}
