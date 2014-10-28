namespace LewisTech.Utils.Query
{
    public interface IQueryProcessor
    {
        TResult Process<TResult>(IQuery<TResult> query);

    }
}