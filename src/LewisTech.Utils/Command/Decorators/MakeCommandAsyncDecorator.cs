using System.Threading.Tasks;

namespace LewisTech.Utils.Command.Decorators
{

    public class MakeCommandAsyncDecorator
    {
        public static MakeCommandAsyncDecorator<TCommand, TResult> Decorate<TCommand, TResult>(ICommandHandler<TCommand, TResult> handler) where TCommand : ICommand<TResult>
        {
            return new MakeCommandAsyncDecorator<TCommand, TResult>(handler);
        }
    }

    public class MakeCommandAsyncDecorator<TCommand, TResult> : IAsyncCommandHandler<TCommand, TResult>, ICommandHandlerDecorator<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        private readonly ICommandHandler<TCommand, TResult> _decorated;

        public MakeCommandAsyncDecorator(ICommandHandler<TCommand, TResult> decorated)
        {
            _decorated = decorated;
        }

        public ICommandHandler<TCommand, TResult> Decorated => _decorated;

        public async Task<TResult> HandleAsync(TCommand command)
        {
            return await Task.Run(() => _decorated.Handle(command));
        }

    }
}
