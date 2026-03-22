using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Infrastructure.Database.Repositories;

public class ProductRepository : IProductRepository
{
    public Task AddAsync(Product product) => throw new NotImplementedException();
    public Task AddRangeAsync(IEnumerable<Product> products) => throw new NotImplementedException();
    public Task UpdateAsync(Product product) => throw new NotImplementedException();
    public Task DeleteAsync(Product product) => throw new NotImplementedException();
}
