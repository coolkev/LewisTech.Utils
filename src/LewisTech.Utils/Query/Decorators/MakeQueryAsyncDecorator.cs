using System.Threading.Tasks;

namespace LewisTech.Utils.Query.Decorators
{

    public class MakeQueryAsyncDecorator<TQuery, TResult> : IAsyncQueryHandler<TQuery, TResult>, IQueryHandlerDecorator<TQuery,TResult> where TQuery : class, IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _decorated;

        public MakeQueryAsyncDecorator(IQueryHandler<TQuery, TResult> decorated)
        {
            _decorated = decorated;
        }

        public IQueryHandler<TQuery, TResult> Decorated => _decorated;

        public async Task<TResult> HandleAsync(TQuery query)
        {
            return await Task.Run(() => _decorated.Handle(query));
        }
    }
}
