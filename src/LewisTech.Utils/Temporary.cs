using System;

namespace LewisTech.Utils
{
    /// <summary>
    /// Provides similar functionality to Lazy&lt;T&gt; but adds support for time based expiration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Temporary<T>
    {
        private readonly Func<T> _factory;
        private readonly TimeSpan _lifetime;
        private readonly object _valueLock = new object();

        private T _value;
        private DateTime _creationTime;

        public Temporary(Func<T> factory, TimeSpan lifetime)
        {
            _factory = factory;
            _lifetime = lifetime;
        }
        public T Value
        {
            get
            {
                DateTime now = DateTime.Now;
                if (_creationTime.Add(_lifetime) < now)
                {
                    lock (_valueLock)
                    {
                        if (_creationTime.Add(_lifetime) < now)
                        {
                            _value = _factory();
                            _creationTime = DateTime.Now;
                        }
                    }
                }

                return _value;
            }
        }

        public void Reset()
        {
            lock (_valueLock)
            {
                _creationTime = DateTime.MinValue;
            }
        }
    }
}