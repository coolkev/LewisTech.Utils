using System;
using System.Collections.Concurrent;
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
            var helper = GetHelper(command);
            return helper.Handle(command);

        }

        [DebuggerStepThrough]
        public virtual async Task<TResult> ProcessAsync<TResult>(ICommand<TResult> command)
        {
            var helper = GetHelper(command);
            return await helper.HandleAsync(command);
        }

        private readonly ConcurrentDictionary<Type, object> s_cache = new ConcurrentDictionary<Type, object>();

        private ICommandHelper<TResult> GetHelper<TResult>(ICommand<TResult> command)
        {
            return (ICommandHelper<TResult>)s_cache.GetOrAdd(
                command.GetType(),
                t =>
                    {
                        var helperType = typeof(CommandHelper<,>).MakeGenericType(t, typeof(TResult));
                        return Activator.CreateInstance(helperType, _resolver);
                    });
        }

        private interface ICommandHelper<TResult>
        {
            TResult Handle(ICommand<TResult> command);

            Task<TResult> HandleAsync(ICommand<TResult> command);
        }

        private class CommandHelper<TCommand, TResult> : ICommandHelper<TResult>
            where TCommand : ICommand<TResult>
        {
            private readonly Func<Type, object> _resolver;

            public CommandHelper(Func<Type, object> resolver)
            {
                _resolver = resolver;
            }

            public TResult Handle(ICommand<TResult> command)
            {
                var handler = Resolve<ICommandHandler<TCommand, TResult>>();
                try
                {
                    return handler.Handle((TCommand)command);
                }
                catch (Exception ex)
                {
                    throw new CommandProcessorHandleException(ex.Message, handler.GetType().DisplayName(), ex);
                }

            }

            public async Task<TResult> HandleAsync(ICommand<TResult> command)
            {
                var handler = Resolve<IAsyncCommandHandler<TCommand, TResult>>();

                try
                {
                    return await handler.HandleAsync((TCommand)command);
                }
                catch (Exception ex)
                {
                    throw new CommandProcessorHandleException(ex.Message, handler.GetType().DisplayName(), ex);
                }
            }

            private T Resolve<T>()
            {
                T result;

                try
                {
                    result = (T)_resolver(typeof(T));
                    Check.NotNull(result, "Handler cannot be null");
                }
                catch (Exception ex)
                {
                    throw new CommandProcessorResolveHandlerException("Unable to resolve handler for type " + typeof(T).DisplayName(), ex);
                }

                return result;

            }
        }
    }

    public class CommandProcessorHandleException : Exception
    {
        public CommandProcessorHandleException(string message, string handlerType, Exception innerException)
            : base(message, innerException)
        {
            HandlerType = handlerType;
        }

        public string HandlerType { get; }
    }

    public class CommandProcessorResolveHandlerException : Exception
    {
        public CommandProcessorResolveHandlerException(string message)
            : base(message)
        {

        }
        public CommandProcessorResolveHandlerException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }

}
