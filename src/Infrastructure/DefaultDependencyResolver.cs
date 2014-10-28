using System;

namespace LewisTech.Utils.Infrastructure
{
    public class DefaultResolver : IResolver
    {
        public T Resolve<T>()
        {
            return Activator.CreateInstance<T>();
        }
    }
}