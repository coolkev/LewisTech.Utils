using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
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
            var helper = GetHelper(query);
            return helper.Handle(query);
        }

        [DebuggerStepThrough]
        public virtual async Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query)
        {
            var helper = GetHelper(query);
            return await helper.HandleAsync(query);
            
        }

        private readonly ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();

        private IQueryHelper<TResult> GetHelper<TResult>(IQuery<TResult> query)
        {
            return (IQueryHelper<TResult>)_cache.GetOrAdd(
                query.GetType(),
                t =>
                    {
                        var helperType = typeof(QueryHelper<,>).MakeGenericType(t, typeof(TResult));
                        return Activator.CreateInstance(helperType, _resolver);
                    });
        }

        private interface IQueryHelper<TResult>
        {
            TResult Handle(IQuery<TResult> query);

            Task<TResult> HandleAsync(IQuery<TResult> query);
        }

        private class QueryHelper<TQuery, TResult> : IQueryHelper<TResult>
            where TQuery : IQuery<TResult>
        {
            private readonly Func<Type, object> _resolver;

            public QueryHelper(Func<Type, object> resolver)
            {
                _resolver = resolver;
            }

            public TResult Handle(IQuery<TResult> query)
            {
                var handler = Resolve<IQueryHandler<TQuery, TResult>>();
                try
                {
                    return handler.Handle((TQuery)query);
                }
                catch (Exception ex)
                {
                    throw new QueryProcessorHandleException(ex.Message, handler.GetType().DisplayName(), ex);
                }

            }

            public async Task<TResult> HandleAsync(IQuery<TResult> query)
            {
                var handler = Resolve<IAsyncQueryHandler<TQuery, TResult>>();
                try
                {
                    return await handler.HandleAsync((TQuery)query);
                }
                catch (Exception ex)
                {
                    throw new QueryProcessorHandleException(ex.Message, handler.GetType().DisplayName(), ex);
                }
            }

            private T Resolve<T>()
            {
                T result;

                try
                {
                    result = (T)_resolver(typeof(T));
                    Check.NotNull(result, "Handler cannot be null");
                }
                catch (Exception ex)
                {
                    throw new QueryProcessorResolveHandlerException("Unable to resolve handler for type " + typeof(T).DisplayName(), ex);
                }

                return result;

            }
        }
    }

    public class QueryProcessorHandleException : Exception
    {
        public string HandlerType { get; }

        public QueryProcessorHandleException(string message, string handlerType, Exception innerException)
            : base(message, innerException)
        {
            HandlerType = handlerType;
        }
    }

    public class QueryProcessorResolveHandlerException : Exception
    {
        public QueryProcessorResolveHandlerException(string message)
            : base(message)
        {

        }
        public QueryProcessorResolveHandlerException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }

}
