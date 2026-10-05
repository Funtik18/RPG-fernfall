using System;

namespace Value
{
    public abstract class Modifier< T > : IReadOnlyValue< T >
    {
        public event Action OnChanged;

        public T Value { get; }

        public Modifier( T value )
        {
            Value = value;
        }
    }
}