using System;
using System.Diagnostics;

namespace LewisTech.Utils.Query
{
    public sealed class QueryProcessor : IQueryProcessor
    {
        private readonly Func<Type, object> _resolver;

        public QueryProcessor(Func<Type, object> resolver)
        {
            _resolver = resolver;
        }

        [DebuggerStepThrough]
        public TResult Process<TResult>(IQuery<TResult> query)
        {
            var handlerType =
                typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = _resolver(handlerType);

            return handler.Handle((dynamic)query);
        }

    }
}
