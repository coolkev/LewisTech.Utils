namespace LewisTech.Utils.Query
{
    
    public interface IQuery<TResult>
    {
    }

    public interface IQueryWithPaging<TResult> : IQuery<TResult>
    {
        
    }
}