using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using LewisTech.Utils.Infrastructure;
using LewisTech.Utils.Query;

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
            var helper = CommandHelper<TResult>.GetHelper(command);
            return helper.Handle(command, _resolver);
        }

        [DebuggerStepThrough]
        public virtual async Task<TResult> ProcessAsync<TResult>(ICommand<TResult> command)
        {
            var helper = CommandHelper<TResult>.GetHelper(command);
            return await helper.HandleAsync(command, _resolver);
        }
        
        private abstract class CommandHelper<TResult>
        {

            private static readonly ConcurrentDictionary<Type, CommandHelper<TResult>> s_cache = new ConcurrentDictionary<Type, CommandHelper<TResult>>();
            public abstract TResult Handle(ICommand<TResult> command, Func<Type, object> resolver);
            public abstract Task<TResult> HandleAsync(ICommand<TResult> command, Func<Type, object> resolver);
            public static CommandHelper<TResult> GetHelper(ICommand<TResult> command)
            {
                return s_cache.GetOrAdd(
                    command.GetType(),
                    t =>
                        {
                            var helperType = typeof(CommandHelper<,>).MakeGenericType(t, typeof(TResult));
                            return (CommandHelper<TResult>)Activator.CreateInstance(helperType);
                        });
            }

        }

        private class CommandHelper<TCommand, TResult> : CommandHelper<TResult> where TCommand : ICommand<TResult>
        {
            public override TResult Handle(ICommand<TResult> command, Func<Type, object> resolver)
            {
                var handler = (ICommandHandler<TCommand, TResult>)resolver(typeof(ICommandHandler<TCommand, TResult>));
                return handler.Handle((TCommand)command);
            }

            public override Task<TResult> HandleAsync(ICommand<TResult> command, Func<Type, object> resolver)
            {
                var handler = (IAsyncCommandHandler<TCommand, TResult>)resolver(typeof(IAsyncCommandHandler<TCommand, TResult>));
                return handler.HandleAsync((TCommand)command);
            }
        }

    }
}
