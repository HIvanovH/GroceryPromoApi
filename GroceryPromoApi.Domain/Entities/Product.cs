namespace GroceryPromoApi.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }

    public int ExternalId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal PriceLev { get; set; }

    public decimal? OldPriceLev { get; set; }

    public string? Discount { get; set; }

    public string? Quantity { get; set; }

    public string? NormalizedQuantity { get; set; }

    public string? Category { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidUntil { get; set; }

    public Guid BrochureId { get; set; }

    public Brochure Brochure { get; set; } = null!;

    public Guid SupermarketId { get; set; }

    public Supermarket Supermarket { get; set; } = null!;
}
