using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Infrastructure.Database.Repositories;

public class FavouriteRepository : IFavouriteRepository
{
    public Task AddAsync(FavouriteProduct favourite) => throw new NotImplementedException();
    public Task UpdateAsync(FavouriteProduct favourite) => throw new NotImplementedException();
    public Task DeleteAsync(FavouriteProduct favourite) => throw new NotImplementedException();
}
