using GroceryPromoApi.Application.Interfaces.Services;
using GroceryPromoApi.Application.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GroceryPromoApi.Controllers;

[ApiController]
[Route("api/v1/sync")]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;
    private readonly PriceBarometerOptions _options;

    public SyncController(ISyncService syncService, IOptions<PriceBarometerOptions> options)
    {
        _syncService = syncService;
        _options = options.Value;
    }

    [HttpPost]
    public async Task<IActionResult> Sync(CancellationToken cancellationToken)
    {
        await _syncService.SyncAsync(cancellationToken);
        return Ok("Sync complete.");
    }

    [HttpGet("debug")]
    public IActionResult Debug()
    {
        return Ok(new
        {
            BaseUrl = _options.BaseUrl,
            ApiKeyLength = _options.ApiKey?.Length,
            ApiKeyStart = _options.ApiKey?.Substring(0, Math.Min(5, _options.ApiKey?.Length ?? 0))
        });
    }
}
