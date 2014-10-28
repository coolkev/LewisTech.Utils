namespace LewisTech.Utils.Infrastructure
{
    public static class StaticResolver
    {
        private static IResolver _current = new DefaultResolver();
        

        public static void SetResolver(IResolver dependencyResolver)
        {
            _current = dependencyResolver;
        }
        public static T Resolve<T>()
        {
            return _current.Resolve<T>();
        }

    }

}
