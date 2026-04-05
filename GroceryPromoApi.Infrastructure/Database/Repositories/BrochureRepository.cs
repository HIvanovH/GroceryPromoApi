using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GroceryPromoApi.Infrastructure.Database.Repositories;

public class BrochureRepository : IBrochureRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BrochureRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HashSet<string>> GetAllBrochureCodesAsync(CancellationToken cancellationToken = default)
    {
        var codes = await _dbContext.Brochures
            .Where(b => b.NextSyncPage == null)
            .Select(b => b.BrochureCode)
            .ToListAsync(cancellationToken);

        return codes.ToHashSet();
    }

    public async Task<List<Brochure>> GetInProgressBrochuresByCodesAsync(IEnumerable<string> codes, Guid supermarketId, CancellationToken cancellationToken = default)
    {
        var codeList = codes.ToList();
        return await _dbContext.Brochures
            .Where(b => b.SupermarketId == supermarketId && b.NextSyncPage != null && codeList.Contains(b.BrochureCode))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Brochure brochure, CancellationToken cancellationToken = default)
    {
        await _dbContext.Brochures.AddAsync(brochure, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Brochure brochure, CancellationToken cancellationToken = default)
    {
        _dbContext.Brochures.Update(brochure);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Brochure brochure, CancellationToken cancellationToken = default)
    {
        _dbContext.Brochures.Remove(brochure);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
