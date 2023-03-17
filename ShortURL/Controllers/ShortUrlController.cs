using Microsoft.AspNetCore.Mvc;

namespace ShortURL.Controllers;

[ApiController]
[Route("[controller]")]
public class ShortUrlController : ControllerBase
{
    private readonly ILogger<ShortUrlController> _logger;
    private readonly IRedis _redis;
    private readonly IUrlRepository _urlRepository;

    public ShortUrlController(ILogger<ShortUrlController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/{shortUrl}")]
    public async Task<IActionResult> Get([FromRoute]string shortUrl)
    {
        var originalLink = await _redis.Get(shortUrl);
        
        if (string.IsNullOrWhiteSpace(originalLink))
        {
            _redis.Lock(shortUrl, async () =>
            {
                originalLink = await _redis.Get(shortUrl);
                if (string.IsNullOrWhiteSpace(originalLink))
                {
                    originalLink = await _urlRepository.Get(shortUrl);
                    await _redis.Save(new Url { OriginalVersion = originalLink, ShortVersion = shortUrl });
                }
            });
        }
        
        if (string.IsNullOrWhiteSpace(originalLink))
            return NotFound();
        
        var originalUrl = "https://www.weareplanet.com";
        var redirect = new RedirectResult(originalLink, true);
        return redirect;

    }

    [HttpPost("")]
    public async Task<IActionResult> Post([FromBody] string originalLink)
    {
        var code = CreateUrlShortVersion();
        var shortLinkAlreadyExists = true;
       
        while (shortLinkAlreadyExists)
        {
            var foundShortUrl = await _urlRepository.Get(code);
            if (!string.IsNullOrWhiteSpace(foundShortUrl))
            {
                code = CreateUrlShortVersion();
                shortLinkAlreadyExists = false;
            }
        }

        Url url = new()
        {
            OriginalVersion = originalLink,
            ShortVersion = $"https://pl.net/{code}",
            Code = code
        };

        await _urlRepository.Save(url);
        await _redis.Save(url);

        return Ok(url.ShortVersion);
    }

    private string CreateUrlShortVersion()
    {
        var characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var code = string.Empty;
        
        Random random = new ();
        for (var i = 0; i < 6; i++)
        {
            code += characters[random.Next(characters.Length)];
        }
        return code;
    }
}

public interface IRedis
{
    Task<string> Get(string link);
    Task Save(Url urlLink);

    void Lock(string key, Action action);
}

public interface IUrlRepository
{
    Task<string> Get(string link);
    Task Save(Url urlLink);
}