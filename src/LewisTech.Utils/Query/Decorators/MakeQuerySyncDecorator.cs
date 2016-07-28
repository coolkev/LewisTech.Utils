using LewisTech.Utils.Async;

namespace LewisTech.Utils.Query.Decorators
{
    public class MakeQuerySyncDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : class, IQuery<TResult>
    {
        private readonly IAsyncQueryHandler<TQuery, TResult> _decorated;

        public MakeQuerySyncDecorator(IAsyncQueryHandler<TQuery, TResult> decorated)
        {
            _decorated = decorated;
        }

        public IAsyncQueryHandler<TQuery, TResult> Decorated => _decorated;

        public TResult Handle(TQuery query)
        {
            return AsyncHelpers.RunSync(() => _decorated.HandleAsync(query));
        }
    }
}