using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task AddRangeAsync(IEnumerable<Product> products);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}
