using GroceryPromoApi.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace GroceryPromoApi.Controllers;

[ApiController]
[Route("api/v1/supermarkets")]
public class SupermarketController : ControllerBase
{
    private readonly ISupermarketService _supermarketService;

    public SupermarketController(ISupermarketService supermarketService)
    {
        _supermarketService = supermarketService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSupermarkets(CancellationToken cancellationToken)
    {
        var result = await _supermarketService.GetAllAsync(cancellationToken);
        return Ok(result);
    }
}
