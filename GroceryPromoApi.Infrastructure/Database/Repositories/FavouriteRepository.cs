using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GroceryPromoApi.Infrastructure.Database.Repositories;

public class FavouriteRepository : IFavouriteRepository
{
    private readonly ApplicationDbContext _dbContext;

    public FavouriteRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(FavouriteProduct favourite, CancellationToken cancellationToken = default)
    {
        await _dbContext.Favourites.AddAsync(favourite, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(FavouriteProduct favourite, CancellationToken cancellationToken = default)
    {
        _dbContext.Favourites.Remove(favourite);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<FavouriteProduct>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Favourites
            .Where(f => f.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<FavouriteProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Favourites
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid userId, string? normalizedName, string? normalizedQuantity, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Favourites.AnyAsync(f =>
            f.UserId == userId &&
            f.NormalizedName == normalizedName &&
            f.NormalizedQuantity == normalizedQuantity, cancellationToken);
    }
}
