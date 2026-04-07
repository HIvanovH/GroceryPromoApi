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
        throw new NotImplementedException("Favourites are being redesigned.");
    }

    public async Task DeleteAsync(FavouriteProduct favourite, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Favourites are being redesigned.");
    }

    public async Task<List<FavouriteProduct>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Favourites are being redesigned.");
    }

    public async Task<FavouriteProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Favourites are being redesigned.");
    }

    public async Task<bool> ExistsAsync(Guid userId, string? normalizedName, string? normalizedQuantity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Favourites are being redesigned.");
    }
}
