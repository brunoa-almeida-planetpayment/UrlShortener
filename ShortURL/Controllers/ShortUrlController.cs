using Microsoft.AspNetCore.Mvc;
using ShortURL.Handlers;
using ShortURL.Handlers.Commands;
using ShortURL.Handlers.Queries;

namespace ShortURL.Controllers;

[ApiController]
[Route("[controller]")]
public class ShortUrlController : ControllerBase
{
    private readonly ILogger<ShortUrlController> _logger;
    private readonly ICommandHandler<CreateShortUrlCommand, CreateShortUrlResponse> _createShortUrlCommandHandler;
    private readonly IQueryHandler<GetUrlQuery, GetUrlResponse> _getUrlQueryHandler;

    public ShortUrlController(ILogger<ShortUrlController> logger,
        ICommandHandler<CreateShortUrlCommand, CreateShortUrlResponse> createShortUrlCommandHandler,
        IQueryHandler<GetUrlQuery, GetUrlResponse> getUrlQueryHandler)
    {
        _logger = logger;
        _createShortUrlCommandHandler = createShortUrlCommandHandler;
        _getUrlQueryHandler = getUrlQueryHandler;
    }

    [HttpGet("/{shortUrl}")]
    public async Task<IActionResult> Get([FromRoute] string shortUrl)
    {
        var query = new GetUrlQuery(shortUrl);
        var response = await _getUrlQueryHandler.Handle(query);

        if (string.IsNullOrWhiteSpace(response.originalUrl))
            return NotFound();

        var redirect = new RedirectResult(response.originalUrl, true);
        return redirect;
    }

    [HttpPost("")]
    public async Task<IActionResult> Post([FromBody] string url)
    {
        var command = new CreateShortUrlCommand(url);
        var response = await _createShortUrlCommandHandler.Handle(command).ConfigureAwait(false);

        return Ok(response);
    }
}