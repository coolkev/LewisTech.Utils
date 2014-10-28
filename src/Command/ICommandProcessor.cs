namespace LewisTech.Utils.Command
{
    public interface ICommandProcessor
    {
        TResult Process<TResult>(ICommand<TResult> command);

    }
}