using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Interfaces.Repositories;

public interface ICatalogueProductRepository
{
    Task<CatalogueProduct?> GetByIdentityAsync(string normalizedName, string? normalizedQuantity, string? category, CancellationToken cancellationToken = default);
    
    Task AddAsync(CatalogueProduct catalogueProduct, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(CatalogueProduct catalogueProduct, CancellationToken cancellationToken = default);
    
    Task UpsertOfferAsync(CatalogueProductOffer offer, CancellationToken cancellationToken = default);
}
