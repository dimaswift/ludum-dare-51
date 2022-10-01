using UnityEngine;
using UnityEngine.Events;

namespace MobRoulette.Core.Events
{
    public class BaseEventWithArgs<T> : BaseEvent
    {
        [SerializeField] protected UnityEvent<T> unityEventWithArgs;

        public void Subscribe(UnityAction<T> action)
        {
            unityEventWithArgs.AddListener(action);
        }

        public void Invoke(T args)
        {
            unityEventWithArgs.Invoke(args);
            unityEvent.Invoke();
        }
        
        public void Unsubscribe(UnityAction<T> action)
        {
            unityEventWithArgs.RemoveListener(action);
        }

        public override void Clear()
        {
            base.Clear();
            unityEventWithArgs.RemoveAllListeners();
        }
    }
}