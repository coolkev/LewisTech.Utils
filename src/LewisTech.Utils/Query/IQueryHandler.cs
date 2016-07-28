using System.Threading.Tasks;

namespace LewisTech.Utils.Query
{

    public interface IQueryHandler
    {

    }
    public interface IQueryHandler<in TQuery, out TResult> : IQueryHandler where TQuery : IQuery<TResult>
    {
        TResult Handle(TQuery query);
    }

    public interface IAsyncQueryHandler<in TQuery, TResult> : IQueryHandler where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }


    public interface IQueryHandlerDecorator<in TQuery, out TResult> : IQueryHandler where TQuery : IQuery<TResult>
    {
        IQueryHandler<TQuery, TResult> Decorated { get; }
    }

    public interface IAsyncQueryHandlerDecorator<in TQuery, TResult> : IQueryHandler where TQuery : IQuery<TResult>
    {
        IAsyncQueryHandler<TQuery, TResult> Decorated { get; }
    }
}
