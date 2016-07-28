using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LewisTech.Utils.Infrastructure;

namespace LewisTech.Utils.Query
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly Func<Type, IQueryHandler> _resolver;

        public QueryProcessor(Func<Type, IQueryHandler> resolver)
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
            var handlerType =
                typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            IQueryHandler handler = _resolver(handlerType);

            return (TResult)((dynamic)handler).Handle((dynamic)query);

        }

        [DebuggerStepThrough]
        public virtual async Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query)
        {
            var handlerType =
                typeof(IAsyncQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            IQueryHandler handler = _resolver(handlerType);

            return (TResult)await((dynamic)handler).HandleAsync((dynamic)query);

        }

    }
}
