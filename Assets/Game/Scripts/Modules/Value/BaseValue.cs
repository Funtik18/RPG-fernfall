using System;

namespace Value
{
    public class BaseValue< T > : IValue< T >
    {
        public event Action OnChanged;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                OnChanged?.Invoke();
            }
        }
        protected T _value;
    }
}