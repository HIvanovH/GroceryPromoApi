namespace GroceryPromoApi.Domain.Entities;

public class PreferredStore
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid SupermarketId { get; set; }
    public Supermarket Supermarket { get; set; } = null!;
}
