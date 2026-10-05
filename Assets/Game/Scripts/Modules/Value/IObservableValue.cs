using System;

namespace Value
{
    public interface IObservableValue
    {
        event Action OnChanged;
    }
}