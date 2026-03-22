using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Infrastructure.Database.Repositories;

public class ProductPendingRepository : IProductPendingRepository
{
    public Task AddAsync(ProductPending product) => throw new NotImplementedException();
    public Task UpdateAsync(ProductPending product) => throw new NotImplementedException();
    public Task DeleteAsync(ProductPending product) => throw new NotImplementedException();
}
