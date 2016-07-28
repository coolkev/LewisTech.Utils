using System.Collections.Generic;
using System.Linq;

namespace LewisTech.Utils.Query.Decorators
{

    public static class QueryHandlerDecoratorExtensions
    {
        
        public static QueryDecoratorHelper<TQuery, TResult> Helper<TQuery, TResult>(this IQueryHandler<TQuery, TResult> handler) where TQuery : class, IQuery<TResult>
        {
            return new QueryDecoratorHelper<TQuery, TResult>(handler);
        }
        public static QueryDecoratorHelper<TQuery, TResult> Helper<TQuery, TResult>(this IAsyncQueryHandler<TQuery, TResult> handler) where TQuery : class, IQuery<TResult>
        {
            return new QueryDecoratorHelper<TQuery, TResult>(handler);
        }
    }
    
    public class QueryDecoratorHelper<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler _handler;

        public QueryDecoratorHelper(IQueryHandler handler)
        {
            _handler = handler;
        }
        
        public IQueryHandler RootHandler()
        {
            return DecoratorChain().LastOrDefault() ?? _handler;
        }

        public IEnumerable<IQueryHandler> DecoratorChain()
        {
            var decorated = GetDecorated(_handler);

            while (decorated != null)
            {
                yield return decorated;
                decorated = GetDecorated(decorated);
            }
        }

        
        private static IQueryHandler GetDecorated(IQueryHandler handler)
        {
            var decorator = handler as IQueryHandlerDecorator<TQuery, TResult>;
            if (decorator != null)
            {
                return decorator.Decorated;
            }

            var asyncDecorator = handler as IAsyncQueryHandlerDecorator<TQuery, TResult>;
            return asyncDecorator?.Decorated;
        }
        
    }

}