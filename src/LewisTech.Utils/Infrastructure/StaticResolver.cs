using System;

namespace LewisTech.Utils.Infrastructure
{
    public static class StaticResolver
    {
        
        public static IServiceProvider Current { get; set; }

        public static T Resolve<T>() where T : class
        {
            return Current.GetService<T>();
        }
    }
}
