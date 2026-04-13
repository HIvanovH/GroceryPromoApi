using GroceryPromoApi.Application.DTOs.Catalogue;
using GroceryPromoApi.Application.Requests.Catalogue;

namespace GroceryPromoApi.Application.Interfaces.Services;

public interface ICatalogueProductService
{
    Task<List<CatalogueProductDTO>> SearchAsync(CatalogueSearchRequest request, CancellationToken cancellationToken = default);
}
