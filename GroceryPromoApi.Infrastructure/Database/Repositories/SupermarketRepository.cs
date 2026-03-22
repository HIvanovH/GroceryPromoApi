using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Infrastructure.Database.Repositories;

public class SupermarketRepository : ISupermarketRepository
{
    public Task AddAsync(Supermarket supermarket) => throw new NotImplementedException();
    public Task UpdateAsync(Supermarket supermarket) => throw new NotImplementedException();
    public Task DeleteAsync(Supermarket supermarket) => throw new NotImplementedException();
}
