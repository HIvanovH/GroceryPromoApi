namespace GroceryPromoApi.Domain.Entities;

public class CatalogueProductOffer
{
    public Guid Id { get; set; }

    public Guid CatalogueProductId { get; set; }

    public CatalogueProduct CatalogueProduct { get; set; } = null!;

    public Guid SupermarketId { get; set; }

    public Supermarket Supermarket { get; set; } = null!;

    public decimal CurrentPriceEur { get; set; }

    public decimal? NormalPriceEur { get; set; }

    public DateTime? PromoValidUntil { get; set; }
}
