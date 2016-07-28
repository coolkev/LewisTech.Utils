using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LewisTech.Utils.Infrastructure;

namespace LewisTech.Utils.Command
{

    public class CommandProcessor : ICommandProcessor
    {
        private readonly Func<Type, ICommandHandler> _resolver;

        public CommandProcessor(Func<Type, ICommandHandler> resolver)
        {
            _resolver = resolver;
        }

        private static ICommandProcessor s_default = new CommandProcessor(t => (ICommandHandler)StaticResolver.Current.GetService(t));

        public static ICommandProcessor Default
        {
            get
            {
                return s_default;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                s_default = value;
            }
        }

        [DebuggerStepThrough]
        public virtual TResult Process<TResult>(ICommand<TResult> command)
        {
            var handlerType =
                typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));

            var handler = _resolver(handlerType);

            return (TResult)((dynamic)handler).Handle((dynamic)command);
        }

        [DebuggerStepThrough]
        public virtual async Task<TResult> ProcessAsync<TResult>(ICommand<TResult> command)
        {
            var handlerType =
                typeof(IAsyncCommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));

            var handler = _resolver(handlerType);

            return (TResult)await ((dynamic)handler).HandleAsync((dynamic)command);
        }

    }
}
