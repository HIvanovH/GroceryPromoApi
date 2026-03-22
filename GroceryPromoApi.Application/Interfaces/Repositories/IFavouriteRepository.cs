using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Interfaces.Repositories;

public interface IFavouriteRepository
{
    Task AddAsync(FavouriteProduct favourite);
    Task UpdateAsync(FavouriteProduct favourite);
    Task DeleteAsync(FavouriteProduct favourite);
}
