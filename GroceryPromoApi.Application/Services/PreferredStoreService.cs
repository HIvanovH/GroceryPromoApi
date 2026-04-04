using GroceryPromoApi.Application.DTOs.PreferredStores;
using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Interfaces.Services;
using GroceryPromoApi.Application.Requests.PreferredStores;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Domain.Exceptions;

namespace GroceryPromoApi.Application.Services;

public class PreferredStoreService : IPreferredStoreService
{
    private readonly IPreferredStoreRepository _preferredStoreRepository;

    public PreferredStoreService(IPreferredStoreRepository preferredStoreRepository)
    {
        _preferredStoreRepository = preferredStoreRepository;
    }

    public async Task<List<PreferredStoreDTO>> GetPreferredStoresAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var stores = await _preferredStoreRepository.GetByUserIdAsync(userId, cancellationToken);

        return stores.Select(s => new PreferredStoreDTO
        {
            Id = s.Id,
            SupermarketId = s.SupermarketId,
            SupermarketName = s.Supermarket.Name,
            SupermarketSlug = s.Supermarket.Slug
        }).ToList();
    }

    public async Task<PreferredStoreDTO> AddPreferredStoreAsync(Guid userId, AddPreferredStoreRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await _preferredStoreRepository.ExistsAsync(userId, request.SupermarketId, cancellationToken);

        if (exists)
            throw new ConflictException("This supermarket is already in your preferred stores.");

        var preferredStore = new PreferredStore
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SupermarketId = request.SupermarketId
        };

        await _preferredStoreRepository.AddAsync(preferredStore, cancellationToken);

        var created = await _preferredStoreRepository.GetByUserAndSupermarketAsync(userId, request.SupermarketId, cancellationToken)
            ?? throw new NotFoundException("Preferred store not found after creation.");

        return new PreferredStoreDTO
        {
            Id = created.Id,
            SupermarketId = created.SupermarketId,
            SupermarketName = created.Supermarket.Name,
            SupermarketSlug = created.Supermarket.Slug
        };
    }

    public async Task RemovePreferredStoreAsync(Guid userId, Guid supermarketId, CancellationToken cancellationToken = default)
    {
        var store = await _preferredStoreRepository.GetByUserAndSupermarketAsync(userId, supermarketId, cancellationToken)
            ?? throw new NotFoundException($"Preferred store for supermarket {supermarketId} not found.");

        await _preferredStoreRepository.DeleteAsync(store, cancellationToken);
    }
}
