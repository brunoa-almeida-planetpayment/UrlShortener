using ShortURL.Models.Interfaces;

namespace ShortURL.Handlers.Commands;

public class CreateShortUrlCommand : ICommand
{
    public CreateShortUrlCommand(string url)
    {
        Url = url;
    }

    public string Url { get; set; }
}

public record CreateShortUrlResponse(string shortUrl);

public class CreateShortUrlCommandHandler : ICommandHandler<CreateShortUrlCommand, CreateShortUrlResponse>
{
    private readonly ILogger<CreateShortUrlCommandHandler> _logger;
    private readonly IRedis _redis;
    private readonly IUrlRepository _urlRepository;

    public CreateShortUrlCommandHandler(ILogger<CreateShortUrlCommandHandler> logger, IRedis redis,
        IUrlRepository urlRepository)
    {
        _logger = logger;
        _redis = redis;
        _urlRepository = urlRepository;
    }

    public async Task<CreateShortUrlResponse> Handle(CreateShortUrlCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Url))
            throw new ArgumentNullException(nameof(command.Url));
        
        Url url = new(command.Url);
        
        var shortLinkAlreadyExists = true;
       
        while (shortLinkAlreadyExists)
        {
            var foundUrl = await _urlRepository.Get(url.Code);
            if (!string.IsNullOrWhiteSpace(foundUrl.Code))
            {
                url.RefreshCode();
                shortLinkAlreadyExists = false;
            }
        }
        
        await _urlRepository.Save(url);
        await _redis.Save(url.Code, url.OriginalUrl);

        return new(url.ShortUrl);
    }
}