namespace ShortURL.Handlers;

public interface IQueryHandler<T, TResult> where T : IQuery
{
    Task<TResult> Handle(T query);
}

