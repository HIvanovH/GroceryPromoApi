using GroceryPromoApi.Application.DTOs.PreferredStores;
using GroceryPromoApi.Application.Requests.PreferredStores;

namespace GroceryPromoApi.Application.Interfaces.Services;

public interface IPreferredStoreService
{
    Task<List<PreferredStoreDTO>> GetPreferredStoresAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PreferredStoreDTO> AddPreferredStoreAsync(Guid userId, AddPreferredStoreRequest request, CancellationToken cancellationToken = default);
    Task RemovePreferredStoreAsync(Guid userId, Guid supermarketId, CancellationToken cancellationToken = default);
}
