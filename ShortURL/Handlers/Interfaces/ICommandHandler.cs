namespace ShortURL.Handlers;

public interface ICommandHandler<T, TResult> where T : ICommand
{
    Task<TResult> Handle(T command);
}