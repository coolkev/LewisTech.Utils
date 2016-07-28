using System;

namespace LewisTech.Utils.Infrastructure
{
    public static class ResolverExtensions
    {

        public static T GetService<T>(this IServiceProvider serviceProvider) where T : class
        {
            return (T)serviceProvider.GetService(typeof(T));
        }
    }
}