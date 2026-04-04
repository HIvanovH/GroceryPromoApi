using GroceryPromoApi.Application.DTOs.Products;
using GroceryPromoApi.Application.Requests.Products;

namespace GroceryPromoApi.Application.Interfaces.Services;

public interface IProductService
{
    Task<List<ProductDTO>> SearchAsync(ProductSearchRequest request, CancellationToken cancellationToken = default);

    Task<ProductDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
