namespace GroceryPromoApi.Application.Interfaces.Services;

public interface IFavouriteService
{
    Task GetFavouritesAsync();
    Task AddFavouriteAsync();
    Task RemoveFavouriteAsync();
}
