using GroceryPromoApi.Application.Models.PriceBarometer;

namespace GroceryPromoApi.Application.Interfaces;

public interface IPriceBarometerClient
{
    Task<List<BrochureResponse>> GetBrochuresAsync(string? supermarketSlug = null, CancellationToken cancellationToken = default);

    Task<List<ProductResponse>> GetProductsBySupermarketAsync(string supermarketSlug, int page, CancellationToken cancellationToken = default);
}
