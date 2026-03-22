namespace GroceryPromoApi.Domain.Entities;

public class ProductPending
{
    public Guid Id { get; set; }
    public int ExternalId { get; set; }
    public string RawName { get; set; } = string.Empty;
    public string? RawQuantity { get; set; }
    public string? NormalizedName { get; set; }
    public string? NormalizedQuantity { get; set; }
    public string Issue { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}
