namespace ShortURL.Models.Interfaces;

public interface IRedis
{
    Task<string> Get(string link);
    Task Save(string key, string value);
    void Lock(string key, Action action);
}