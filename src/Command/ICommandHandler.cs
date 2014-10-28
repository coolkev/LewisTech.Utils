namespace LewisTech.Utils.Command
{

    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand,DefaultCommandResult> where TCommand : ICommand<DefaultCommandResult>
    {
        
    }
    public interface ICommandHandler<in TCommand, out TResult> where TCommand : ICommand<TResult>
    {
        TResult Handle(TCommand command);
    }
}