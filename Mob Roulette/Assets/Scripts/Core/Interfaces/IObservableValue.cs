using System;

namespace MobRoulette.Core.Interfaces
{
    public interface IObservableValue<T>
    {
        T Value { get; }
        void Subscribe(Action<T> action);
        void Unsubscribe(Action<T> action);
    }
}