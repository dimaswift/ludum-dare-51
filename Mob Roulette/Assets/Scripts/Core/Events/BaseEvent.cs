using UnityEngine;
using UnityEngine.Events;

namespace MobRoulette.Core.Events
{
    [CreateAssetMenu(menuName = "Create BaseEvent", fileName = "BaseEvent", order = 0)]
    public class BaseEvent : ScriptableObject
    {
        [SerializeField] protected UnityEvent unityEvent;

        public void Subscribe(UnityAction action)
        {
            unityEvent.AddListener(action);
        }

        public void Invoke()
        {
            unityEvent.Invoke();
        }

        public void Unsubscribe(UnityAction action)
        {
            unityEvent.RemoveListener(action);
        }

        public virtual void Clear()
        {
            unityEvent.RemoveAllListeners();
        }
    }
}