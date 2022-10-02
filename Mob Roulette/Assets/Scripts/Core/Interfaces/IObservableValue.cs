using System;

namespace MobRoulette.Core.Interfaces
{
    public interface IObservableValue<out T>
    {
        T Value { get; }
        void Subscribe(Action<T> action, bool executeImmediately = false);
        void Unsubscribe(Action<T> action);
    }
}