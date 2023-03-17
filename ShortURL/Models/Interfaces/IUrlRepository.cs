namespace ShortURL.Models.Interfaces;

public interface IUrlRepository
{
    Task<Url> Get(string link);
    Task Save(Url urlLink);
}