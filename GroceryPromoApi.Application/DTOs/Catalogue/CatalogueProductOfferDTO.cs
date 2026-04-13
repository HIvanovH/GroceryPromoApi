namespace GroceryPromoApi.Application.DTOs.Catalogue;

public class CatalogueProductOfferDTO
{
    public Guid SupermarketId { get; set; }

    public string SupermarketName { get; set; } = string.Empty;

    public decimal CurrentPriceEur { get; set; }

    public decimal? NormalPriceEur { get; set; }

    public bool IsOnPromo { get; set; }

    public DateTime? PromoValidUntil { get; set; }
}
