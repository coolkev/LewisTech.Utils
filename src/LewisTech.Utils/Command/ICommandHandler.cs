using System.Threading.Tasks;

namespace LewisTech.Utils.Command
{

    public interface ICommandHandler
    {

    }

    public interface ICommandHandler<in TCommand, out TResult> : ICommandHandler
        where TCommand : ICommand<TResult>
    {
        TResult Handle(TCommand command);
    }

    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, DefaultCommandResult>
        where TCommand : ICommand<DefaultCommandResult>
    {

    }

    public interface IAsyncCommandHandler<in TCommand, TResult> : ICommandHandler
        where TCommand : ICommand<TResult>
    {
        Task<TResult> HandleAsync(TCommand command);

    }
    
    //public interface ICommandHandlerDecorator : ICommandHandler
    //{
    //    ICommandHandler Decorated { get; }
    //}

    public interface ICommandHandlerDecorator<in TCommand, out TResult> : ICommandHandler
        where TCommand : ICommand<TResult>
    {
        ICommandHandler<TCommand, TResult> Decorated { get; }
    }

    public interface IAsyncCommandHandlerDecorator<in TCommand, TResult> : ICommandHandler
        where TCommand : ICommand<TResult>
    {
        IAsyncCommandHandler<TCommand, TResult> Decorated { get; }
    }
}
