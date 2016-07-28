using System;

namespace LewisTech.Utils
{
    public class ResettableLazy<T>
    {

        private readonly object _lock = new object();
        private readonly Func<T> _valueFactory;
        private Boxed _value;

        public ResettableLazy(Func<T> valueFactory)
        {
            _valueFactory = valueFactory;
        }

        public T Value
        {
            get
            {

                var value = _value;

                if (value == null)
                {
                    lock (_lock)
                    {
                        value = _value;
                        if (_value == null)
                        {
                            value = new Boxed(_valueFactory());
                            _value = value;

                        }

                    }
                }
                return value.Value;
            }
        }

        public void Reset()
        {
            lock (_lock)
            {
                _value = null;
            }
        }

        private class Boxed
        {
            internal Boxed(T value)
            {
                Value = value;
            }
            internal T Value { get; }
        }
    }
}