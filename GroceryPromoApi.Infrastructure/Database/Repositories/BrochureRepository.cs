using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Infrastructure.Database.Repositories;

public class BrochureRepository : IBrochureRepository
{
    public Task AddAsync(Brochure brochure) => throw new NotImplementedException();
    public Task UpdateAsync(Brochure brochure) => throw new NotImplementedException();
    public Task DeleteAsync(Brochure brochure) => throw new NotImplementedException();
}
