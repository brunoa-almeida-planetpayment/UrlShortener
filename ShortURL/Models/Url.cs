namespace ShortURL;

public class Url
{
    private const string Characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public Url(string originalVersion)
    {
        OriginalUrl = originalVersion;
        CreateCode();
    }
    public string OriginalUrl { get; }
    public string ShortUrl => $"https://pl.net/{Code}";
    public string Code { get; private set; }
    
    private void CreateCode()
    {
        Random random = new();
        for (var i = 0; i < 6; i++)
        {
            Code += Characters[random.Next(Characters.Length)];
        }
    }

    public void RefreshCode() => CreateCode();
}