using GroceryPromoApi.Application.DTOs.Catalogue;
using GroceryPromoApi.Application.Requests.Favourites;

namespace GroceryPromoApi.Application.Interfaces.Services;

public interface IFavouriteService
{
    Task<List<CatalogueProductDTO>> GetFavouritesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<CatalogueProductDTO> AddFavouriteAsync(Guid userId, AddFavouriteRequest request, CancellationToken cancellationToken = default);

    Task RemoveFavouriteAsync(Guid userId, Guid catalogueProductId, CancellationToken cancellationToken = default);
}
