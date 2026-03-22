using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Infrastructure.Database.Repositories;

public class SyncStateRepository : ISyncStateRepository
{
    public Task AddAsync(SyncState syncState) => throw new NotImplementedException();
    public Task UpdateAsync(SyncState syncState) => throw new NotImplementedException();
    public Task DeleteAsync(SyncState syncState) => throw new NotImplementedException();
}
