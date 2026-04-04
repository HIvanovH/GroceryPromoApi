using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GroceryPromoApi.Infrastructure.Database.Repositories;

public class PreferredStoreRepository : IPreferredStoreRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PreferredStoreRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<PreferredStore>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PreferredStores
            .Include(p => p.Supermarket)
            .Where(p => p.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid supermarketId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PreferredStores
            .AnyAsync(p => p.UserId == userId && p.SupermarketId == supermarketId, cancellationToken);
    }

    public async Task AddAsync(PreferredStore preferredStore, CancellationToken cancellationToken = default)
    {
        await _dbContext.PreferredStores.AddAsync(preferredStore, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(PreferredStore preferredStore, CancellationToken cancellationToken = default)
    {
        _dbContext.PreferredStores.Remove(preferredStore);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PreferredStore?> GetByUserAndSupermarketAsync(Guid userId, Guid supermarketId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PreferredStores
            .FirstOrDefaultAsync(p => p.UserId == userId && p.SupermarketId == supermarketId, cancellationToken);
    }
}
