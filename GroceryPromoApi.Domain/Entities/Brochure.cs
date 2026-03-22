namespace GroceryPromoApi.Domain.Entities;

public class Brochure
{
    public Guid Id { get; set; }
    public string BrochureCode { get; set; } = string.Empty;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public DateTime SyncedAt { get; set; }

    public Guid SupermarketId { get; set; }
    public Supermarket Supermarket { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
