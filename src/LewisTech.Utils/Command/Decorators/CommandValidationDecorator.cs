using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LewisTech.Utils.Command.Decorators
{

    public class CommandValidationDecorator
    {
        public static CommandValidationDecorator<TCommand> Decorate<TCommand>(ICommandHandler<TCommand> handler) where TCommand : ICommand
        {
            return new CommandValidationDecorator<TCommand>(handler);
        }
        public static CommandValidationDecorator<TCommand, TResult> Decorate<TCommand, TResult>(ICommandHandler<TCommand, TResult> handler) where TCommand : ICommand<TResult>
        {
            return new CommandValidationDecorator<TCommand, TResult>(handler);
        }
        public static AsyncCommandValidationDecorator<TCommand, TResult> Decorate<TCommand, TResult>(IAsyncCommandHandler<TCommand, TResult> handler) where TCommand : ICommand<TResult>
        {
            return new AsyncCommandValidationDecorator<TCommand, TResult>(handler);
        }
    }

    public class CommandValidationDecorator<TCommand> : CommandValidationDecorator<TCommand, DefaultCommandResult>, ICommandHandler<TCommand> where TCommand : ICommand
    {
        public CommandValidationDecorator(ICommandHandler<TCommand> decoratedHandler) : base(decoratedHandler)
        {
        }
    }

    public class CommandValidationDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>, ICommandHandlerDecorator<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        private readonly ICommandHandler<TCommand, TResult> _decoratedHandler;
        public CommandValidationDecorator(ICommandHandler<TCommand, TResult> decoratedHandler)
        {
            _decoratedHandler = decoratedHandler;
        }

        public ICommandHandler<TCommand, TResult> Decorated => _decoratedHandler;

        public TResult Handle(TCommand command)
        {

            var context = new ValidationContext(command);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(command, context, results, true);

            if (!isValid)
            {
                throw new ValidationException("Validation Errors: " + string.Join(", ", results.Select(v => v.ErrorMessage)));
            }

            return _decoratedHandler.Handle(command);
        }
    }


    public class AsyncCommandValidationDecorator<TCommand, TResult> : IAsyncCommandHandler<TCommand, TResult>, IAsyncCommandHandlerDecorator<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        private readonly IAsyncCommandHandler<TCommand, TResult> _decoratedHandler;
        public AsyncCommandValidationDecorator(IAsyncCommandHandler<TCommand, TResult> decoratedHandler)
        {
            _decoratedHandler = decoratedHandler;
        }

        public IAsyncCommandHandler<TCommand, TResult> Decorated => _decoratedHandler;

        public async Task<TResult> HandleAsync(TCommand command)
        {

            var context = new ValidationContext(command);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(command, context, results, true);

            if (!isValid)
            {
                throw new ValidationException("Validation Errors: " + string.Join(", ", results.Select(v => v.ErrorMessage)));
            }

            return await _decoratedHandler.HandleAsync(command);
        }
    }
}
