using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Interfaces.Repositories;

public interface IProductPendingRepository
{
    Task AddAsync(ProductPending product);
    Task UpdateAsync(ProductPending product);
    Task DeleteAsync(ProductPending product);
}
