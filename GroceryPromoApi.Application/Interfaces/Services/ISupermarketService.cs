using GroceryPromoApi.Application.DTOs.Supermarkets;

namespace GroceryPromoApi.Application.Interfaces.Services;

public interface ISupermarketService
{
    Task<List<SupermarketDTO>> GetAllAsync(CancellationToken cancellationToken = default);
}
