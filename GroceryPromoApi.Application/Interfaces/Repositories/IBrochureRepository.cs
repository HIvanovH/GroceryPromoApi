using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Interfaces.Repositories;

public interface IBrochureRepository
{
    Task AddAsync(Brochure brochure);
    Task UpdateAsync(Brochure brochure);
    Task DeleteAsync(Brochure brochure);
}
