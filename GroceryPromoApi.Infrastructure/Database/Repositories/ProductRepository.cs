using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GroceryPromoApi.Infrastructure.Database.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Product>> SearchAsync(string? name, Guid? supermarketId, string? category, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Products
            .Include(p => p.Supermarket)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.NormalizedName.Contains(name.ToLower()));

        if (supermarketId.HasValue)
            query = query.Where(p => p.SupermarketId == supermarketId.Value);

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category == category);

        return await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Include(p => p.Supermarket)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default) 
    {
        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddRangeAsync(products);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<HashSet<int>> GetExternalIdsByBrochureIdsAsync(IEnumerable<Guid> brochureIds, CancellationToken cancellationToken = default)
    {
        var ids = brochureIds.ToList();
        var externalIds = await _dbContext.Products
            .Where(p => ids.Contains(p.BrochureId))
            .Select(p => p.ExternalId)
            .ToListAsync(cancellationToken);

        return externalIds.ToHashSet();
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
         _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public  async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
