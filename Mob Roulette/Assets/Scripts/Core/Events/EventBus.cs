using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MobRoulette.Core.Events
{
    public static class EventBus
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            events = new();
        }
        
        private static Dictionary<Type, BaseEvent> events = new ();

        private static BaseEventWithArgs<T2> Get<T1, T2>() where T1 : BaseEventWithArgs<T2>
        {
            if (!events.TryGetValue(typeof(T1), out var evt))
            {
                evt = Resources.Load<BaseEvent>($"Events/{typeof(T1).Name}");
                events.Add(typeof(T1), evt);
            }

            if (evt == null)
            {
                throw new Exception($"Event not found. Should be in Resources/Events/{typeof(T1).Name}");
            }
            
            var generic = evt as BaseEventWithArgs<T2>;
            return generic;
        }

        private static BaseEvent Get<T1>() where T1 : BaseEvent
        {
            if (!events.TryGetValue(typeof(T1), out var evt))
            {
                evt = Resources.Load<BaseEvent>($"Events/{typeof(T1).Name}");
                events.Add(typeof(T1), evt);
            }
            return evt;
        }
        
        public static void Subscribe<T1, T2>(UnityAction<T2> action) where T1 : BaseEventWithArgs<T2>
        {
            var generic = Get<T1, T2>();
            generic.Subscribe(action);
        }

        public static void Subscribe<T1>(UnityAction action) where T1 : BaseEvent
        {
            var generic = Get<T1>();
            generic.Subscribe(action);
        }

        public static void Raise<T1, T2>(T2 args) where T1 : BaseEventWithArgs<T2>
        {
            var generic = Get<T1,T2>();
            generic.Invoke(args);
        }

        public static void Raise<T1>() where T1 : BaseEvent
        {
            var generic = Get<T1>();
            generic.Invoke();
        }

        public static void Unsubscribe<T1, T2>(UnityAction<T2> action) where T1 : BaseEventWithArgs<T2>
        {
            var generic = Get<T1, T2>();
            generic.Unsubscribe(action);
        }

        public static void Unsubscribe<T1>(UnityAction action) where T1 : BaseEvent
        {
            var generic = Get<T1>();
            generic.Unsubscribe(action);
        }

        public static void Clear<T1>()
        {
            if (events.TryGetValue(typeof(T1), out var evt))
            {
                evt.Clear();
            }
        }
    }
}