using GroceryPromoApi.Application.DTOs.Favorites;
using GroceryPromoApi.Application.Requests.Favourites;

namespace GroceryPromoApi.Application.Interfaces.Services;

public interface IFavouriteService
{
    Task<List<FavouriteProductDTO>> GetFavouritesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<FavouriteProductDTO> AddFavouriteAsync(Guid userId, AddFavouriteRequest request, CancellationToken cancellationToken = default);

    Task RemoveFavouriteAsync(Guid userId, Guid favouriteId, CancellationToken cancellationToken = default);
}
