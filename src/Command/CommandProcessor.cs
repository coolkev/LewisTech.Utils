using System;
using System.Diagnostics;

namespace LewisTech.Utils.Command
{
    public sealed class CommandProcessor : ICommandProcessor
    {
        private readonly Func<Type, object> _resolver;

        public CommandProcessor(Func<Type, object> resolver)
        {
            _resolver = resolver;
        }

        [DebuggerStepThrough]
        public TResult Process<TResult>(ICommand<TResult> command)
        {
            var handlerType =
                typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));

            dynamic handler = _resolver(handlerType);

            return handler.Handle((dynamic)command);
        }

    }
}
