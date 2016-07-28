using LewisTech.Utils.Async;

namespace LewisTech.Utils.Command.Decorators
{
    
    public class MakeCommandSyncDecorator
    {
        public static MakeCommandSyncDecorator<TCommand, TResult> Decorate<TCommand, TResult>(IAsyncCommandHandler<TCommand, TResult> handler) where TCommand : ICommand<TResult>
        {
            return new MakeCommandSyncDecorator<TCommand, TResult>(handler);
        }
    }

    public class MakeCommandSyncDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>, IAsyncCommandHandlerDecorator<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        private readonly IAsyncCommandHandler<TCommand, TResult> _decorated;

        public MakeCommandSyncDecorator(IAsyncCommandHandler<TCommand, TResult> decorated)
        {
            _decorated = decorated;
        }

        public IAsyncCommandHandler<TCommand, TResult> Decorated => _decorated;

        public TResult Handle(TCommand command)
        {
            return AsyncHelpers.RunSync(() => _decorated.HandleAsync(command));
        }
    }
}