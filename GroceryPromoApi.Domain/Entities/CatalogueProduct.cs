namespace GroceryPromoApi.Domain.Entities;

public class CatalogueProduct
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;

    public string? NormalizedQuantity { get; set; }

    public string? Category { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime LastSeenAt { get; set; }

    public ICollection<CatalogueProductOffer> Offers { get; set; } = new List<CatalogueProductOffer>();
}
