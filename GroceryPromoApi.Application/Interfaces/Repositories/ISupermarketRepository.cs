using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Interfaces.Repositories;

public interface ISupermarketRepository
{
    Task AddAsync(Supermarket supermarket);
    Task UpdateAsync(Supermarket supermarket);
    Task DeleteAsync(Supermarket supermarket);
}
