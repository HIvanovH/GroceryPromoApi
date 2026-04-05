using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GroceryPromoApi.Infrastructure.Database.Repositories;

public class SupermarketRepository : ISupermarketRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SupermarketRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Supermarket>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Supermarkets.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Supermarket supermarket, CancellationToken cancellationToken = default)
    {
        await _dbContext.Supermarkets.AddAsync(supermarket, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Supermarket supermarket, CancellationToken cancellationToken = default)
    {
        _dbContext.Supermarkets.Update(supermarket);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Supermarket supermarket, CancellationToken cancellationToken = default)
    {
        _dbContext.Supermarkets.Remove(supermarket);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Supermarket?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Supermarkets
            .FirstOrDefaultAsync(s => s.Slug == slug, cancellationToken);
    }
}
