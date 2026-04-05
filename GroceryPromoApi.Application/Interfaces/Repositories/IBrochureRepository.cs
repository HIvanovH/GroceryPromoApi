using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Interfaces.Repositories;

public interface IBrochureRepository
{
    Task<HashSet<string>> GetAllBrochureCodesAsync(CancellationToken cancellationToken = default);

    Task<List<Brochure>> GetInProgressBrochuresByCodesAsync(IEnumerable<string> codes, Guid supermarketId, CancellationToken cancellationToken = default);

    Task AddAsync(Brochure brochure, CancellationToken cancellationToken = default);

    Task UpdateAsync(Brochure brochure, CancellationToken cancellationToken = default);

    Task DeleteAsync(Brochure brochure, CancellationToken cancellationToken = default);
}
