using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Interfaces.Repositories;

public interface IPreferredStoreRepository
{
    Task<List<PreferredStore>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid userId, Guid supermarketId, CancellationToken cancellationToken = default);
    Task AddAsync(PreferredStore preferredStore, CancellationToken cancellationToken = default);
    Task DeleteAsync(PreferredStore preferredStore, CancellationToken cancellationToken = default);
    Task<PreferredStore?> GetByUserAndSupermarketAsync(Guid userId, Guid supermarketId, CancellationToken cancellationToken = default);
}
