using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using LewisTech.Utils.Infrastructure;

namespace LewisTech.Utils.Query
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly Func<Type, object> _resolver;

        public QueryProcessor(Func<Type, object> resolver)
        {
            _resolver = resolver;
        }

        private static IQueryProcessor s_default = new QueryProcessor(t => (IQueryHandler)StaticResolver.Current.GetService(t));

        public static IQueryProcessor Default
        {
            get
            {
                return s_default;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                s_default = value;
            }
        }
        [DebuggerStepThrough]
        public virtual TResult Process<TResult>(IQuery<TResult> query)
        {
            var helper = QueryHelper<TResult>.GetHelper(query);
            return helper.Handle(query, _resolver);
        }
        
        [DebuggerStepThrough]
        public virtual async Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query)
        {
            var helper = QueryHelper<TResult>.GetHelper(query);
            return await helper.HandleAsync(query, _resolver);
        }

        private abstract class QueryHelper<TResult>
        {

            private static readonly ConcurrentDictionary<Type, QueryHelper<TResult>> s_cache = new ConcurrentDictionary<Type, QueryHelper<TResult>>();
            public abstract TResult Handle(IQuery<TResult> query, Func<Type, object> resolver);
            public abstract Task<TResult> HandleAsync(IQuery<TResult> query, Func<Type, object> resolver);
            public static QueryHelper<TResult> GetHelper(IQuery<TResult> query)
            {
                return s_cache.GetOrAdd(
                    query.GetType(),
                    t =>
                        {
                            var helperType = typeof(QueryHelper<,>).MakeGenericType(t, typeof(TResult));
                            return (QueryHelper<TResult>)Activator.CreateInstance(helperType);
                        });
            }

        }

        private class QueryHelper<TQuery, TResult> : QueryHelper<TResult> where TQuery : IQuery<TResult>
        {
            public override TResult Handle(IQuery<TResult> query, Func<Type, object> resolver)
            {
                var handler = (IQueryHandler<TQuery, TResult>)resolver(typeof(IQueryHandler<TQuery, TResult>));
                return handler.Handle((TQuery)query);
            }

            public override Task<TResult> HandleAsync(IQuery<TResult> query, Func<Type, object> resolver)
            {
                var handler = (IAsyncQueryHandler<TQuery, TResult>)resolver(typeof(IAsyncQueryHandler<TQuery, TResult>));
                return handler.HandleAsync((TQuery)query);
            }
        }

    }

}
