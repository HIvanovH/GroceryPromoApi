using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GroceryPromoApi.Infrastructure.Database.Repositories;

public class CatalogueProductRepository : ICatalogueProductRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CatalogueProductRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CatalogueProduct?> GetByIdentityAsync(string normalizedName, string? normalizedQuantity, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CatalogueProducts
            .FirstOrDefaultAsync(c =>
                c.NormalizedName == normalizedName &&
                c.NormalizedQuantity == normalizedQuantity,
            cancellationToken);
    }

    public async Task<List<CatalogueProduct>> SearchAsync(string? name, string? category, Guid? supermarketId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.CatalogueProducts
            .Include(c => c.Offers)
                .ThenInclude(o => o.Supermarket)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(c => c.NormalizedName.Contains(name.ToLower().Trim()));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(c => c.Category == category);

        if (supermarketId.HasValue)
            query = query.Where(c => c.Offers.Any(o => o.SupermarketId == supermarketId.Value));

        return await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CatalogueProduct catalogueProduct, CancellationToken cancellationToken = default)
    {
        await _dbContext.CatalogueProducts.AddAsync(catalogueProduct, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(CatalogueProduct catalogueProduct, CancellationToken cancellationToken = default)
    {
        _dbContext.CatalogueProducts.Update(catalogueProduct);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertOfferAsync(CatalogueProductOffer offer, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.CatalogueProductOffers
            .FirstOrDefaultAsync(o =>
                o.CatalogueProductId == offer.CatalogueProductId &&
                o.SupermarketId == offer.SupermarketId,
            cancellationToken);

        if (existing is null)
        {
            await _dbContext.CatalogueProductOffers.AddAsync(offer, cancellationToken);
        }
        else
        {
            existing.CurrentPriceEur = offer.CurrentPriceEur;
            existing.NormalPriceEur = offer.NormalPriceEur;
            existing.PromoValidUntil = offer.PromoValidUntil;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<CatalogueProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.CatalogueProducts
                        .Include(c=>c.Offers)
                            .ThenInclude(o => o.Supermarket)
                        .FirstOrDefaultAsync(c=>c.Id == id, cancellationToken);
    }
}
