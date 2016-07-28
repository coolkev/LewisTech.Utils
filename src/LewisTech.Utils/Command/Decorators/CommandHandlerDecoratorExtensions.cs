using System.Collections.Generic;
using System.Linq;

namespace LewisTech.Utils.Command.Decorators
{

    public static class CommandHandlerDecoratorExtensions
    {
        
        public static CommandDecoratorHelper<TCommand, TResult> Helper<TCommand, TResult>(this ICommandHandler<TCommand, TResult> handler) where TCommand : ICommand<TResult>
        {
            return new CommandDecoratorHelper<TCommand, TResult>(handler);
        }
        public static CommandDecoratorHelper<TCommand, TResult> Helper<TCommand, TResult>(this IAsyncCommandHandler<TCommand, TResult> handler) where TCommand : ICommand<TResult>
        {
            return new CommandDecoratorHelper<TCommand, TResult>(handler);
        }
    }
    
    public class CommandDecoratorHelper<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        private readonly ICommandHandler _handler;

        public CommandDecoratorHelper(ICommandHandler handler)
        {
            _handler = handler;
        }
        
        public ICommandHandler RootHandler()
        {
            return DecoratorChain().LastOrDefault() ?? _handler;
        }

        public IEnumerable<ICommandHandler> DecoratorChain()
        {
            var decorated = GetDecorated(_handler);

            while (decorated != null)
            {
                yield return decorated;
                decorated = GetDecorated(decorated);
            }
        }

        
        private static ICommandHandler GetDecorated(ICommandHandler handler)
        {
            var decorator = handler as ICommandHandlerDecorator<TCommand, TResult>;
            if (decorator != null)
            {
                return decorator.Decorated;
            }

            var asyncDecorator = handler as IAsyncCommandHandlerDecorator<TCommand, TResult>;
            return asyncDecorator?.Decorated;
        }
        
    }

}