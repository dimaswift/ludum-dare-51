
using System;
using MobRoulette.Core.Interfaces;

namespace MobRoulette.Core.Utils
{
    public sealed class Observable<T> : IObservableValue<T> where T : struct
    {
        private T value;
        public T Value => value;
        
        public void Subscribe(Action<T> action)
        {
            OnChange += action;
        }

        public void Unsubscribe(Action<T> action)
        {
            OnChange -= action;
        }

        public event System.Action<T> OnChange = v => {};
        
        public void Set(T newValue)
        {
            if (!value.Equals(newValue))
            {
                value = newValue;
                OnChange(value);
            }
        }
        
        public Observable(T defaultValue)
        {
            value = defaultValue;
        }

        private Observable()
        {
 
        }

        public static implicit operator T(Observable<T> observable)
        {
            return observable.Value;
        }
        
        public override string ToString()
        {
            return value.ToString();
        }
        
        public override bool Equals(object obj)
        {
            return value.Equals(obj);
        }
        
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

    }
}