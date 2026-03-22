namespace GroceryPromoApi.Domain.Entities;

public class FavouriteProduct
{
    public Guid Id { get; set; }
    public string Type { get; set; } = "Product";
    public string? NormalizedName { get; set; }
    public string? NormalizedQuantity { get; set; }
    public string? Category { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
