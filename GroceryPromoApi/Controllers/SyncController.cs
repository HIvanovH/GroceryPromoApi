using GroceryPromoApi.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryPromoApi.Controllers;

[ApiController]
[Route("api/v1/sync")]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;

    public SyncController(ISyncService syncService)
    {
        _syncService = syncService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Sync(CancellationToken cancellationToken)
    {
        await _syncService.SyncAsync(cancellationToken);
        return Ok("Sync complete.");
    }
}
