using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Interfaces.Repositories;

public interface ISyncStateRepository
{
    Task AddAsync(SyncState syncState);
    Task UpdateAsync(SyncState syncState);
    Task DeleteAsync(SyncState syncState);
}
