using GroceryPromoApi.Domain.Entities;
using System.Threading;

namespace GroceryPromoApi.Application.Interfaces.Repositories;

public interface IFavouriteRepository
{
    Task AddAsync(FavouriteProduct favourite, CancellationToken cancellationToken = default);

    Task DeleteAsync(FavouriteProduct favourite, CancellationToken cancellationToken = default);

    Task<List<FavouriteProduct>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<FavouriteProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid userId, string? normalizedName, string? normalizedQuantity, CancellationToken cancellationToken = default);
}
