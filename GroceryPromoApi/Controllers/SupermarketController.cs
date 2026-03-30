using GroceryPromoApi.Application.DTOs.Supermarkets;
using GroceryPromoApi.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GroceryPromoApi.Controllers;

[ApiController]
[Route("api/v1/supermarkets")]
public class SupermarketController : ControllerBase
{
    private readonly ISupermarketRepository _supermarketRepository;

    public SupermarketController(ISupermarketRepository supermarketRepository)
    {
        _supermarketRepository = supermarketRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSupermarkets(CancellationToken cancellationToken)
    {
        var supermarkets = await _supermarketRepository.GetAllAsync(cancellationToken);
        var result = supermarkets.Select(s => new SupermarketDTO
        {
            Id = s.Id,
            Name = s.Name,
            Slug = s.Slug
        }).ToList();

        return Ok(result);
    }
}
