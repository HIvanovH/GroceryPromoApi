using GroceryPromoApi.Application.Interfaces.Services;
using GroceryPromoApi.Application.Requests.PreferredStores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GroceryPromoApi.Controllers;

[ApiController]
[Route("api/v1/preferred-stores")]
[Authorize]
public class PreferredStoresController : ControllerBase
{
    private readonly IPreferredStoreService _preferredStoreService;

    public PreferredStoresController(IPreferredStoreService preferredStoreService)
    {
        _preferredStoreService = preferredStoreService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPreferredStores(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _preferredStoreService.GetPreferredStoresAsync(userId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddPreferredStore([FromBody] AddPreferredStoreRequest request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _preferredStoreService.AddPreferredStoreAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(GetPreferredStores), result);
    }

    [HttpDelete("{supermarketId:guid}")]
    public async Task<IActionResult> RemovePreferredStore(Guid supermarketId, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _preferredStoreService.RemovePreferredStoreAsync(userId, supermarketId, cancellationToken);
        return NoContent();
    }
}
