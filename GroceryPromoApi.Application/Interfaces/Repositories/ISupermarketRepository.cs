using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Interfaces.Repositories;

public interface ISupermarketRepository
{
    Task<List<Supermarket>> GetAllAsync(CancellationToken cancellationToken = default);
   
    Task AddAsync(Supermarket supermarket, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(Supermarket supermarket, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Supermarket supermarket, CancellationToken cancellationToken = default);
}
