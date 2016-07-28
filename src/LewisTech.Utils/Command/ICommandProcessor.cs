using System.Threading.Tasks;

namespace LewisTech.Utils.Command
{
    public interface ICommandProcessor
    {
        TResult Process<TResult>(ICommand<TResult> command);
        Task<TResult> ProcessAsync<TResult>(ICommand<TResult> command);

    }
}