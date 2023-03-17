using ShortURL.Models.Interfaces;

namespace ShortURL.Handlers.Queries;

public class GetUrlQuery : IQuery
{
    public GetUrlQuery(string url)
    {
        Url = url;
    }

    public string Url { get; set; }
}

public record GetUrlResponse(string originalUrl);

public class GetUrlQueryHandler : IQueryHandler<GetUrlQuery, GetUrlResponse>
{
    private readonly ILogger<GetUrlQueryHandler> _logger;
    private readonly IRedis _redis;
    private readonly IUrlRepository _urlRepository;

    public GetUrlQueryHandler(ILogger<GetUrlQueryHandler> logger, IRedis redis,
        IUrlRepository urlRepository)
    {
        _logger = logger;
        _redis = redis;
        _urlRepository = urlRepository;
    }

    public async Task<GetUrlResponse> Handle(GetUrlQuery query)
    {
        var originalUrl = await _redis.Get(query.Url);

        if (string.IsNullOrWhiteSpace(originalUrl))
        {
            _redis.Lock(query.Url, async () =>
            {
                originalUrl = await _redis.Get(query.Url);
                if (string.IsNullOrWhiteSpace(originalUrl))
                {
                    var url = await _urlRepository.Get(query.Url);
                    await _redis.Save(url.Code, url.OriginalUrl);
                }
            });
        }

        return new(originalUrl);
    }
}