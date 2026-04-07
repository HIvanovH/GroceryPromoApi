namespace GroceryPromoApi.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string? PasswordHash { get; set; }

    public string? GoogleId { get; set; }

    public string Role { get; set; } = "User";

    public int FailedLoginAttempts { get; set; } = 0;

    public DateTime? LockoutUntil { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<CatalogueProduct> Favourites { get; set; } = new List<CatalogueProduct>();

    public ICollection<PreferredStore> PreferredStores { get; set; } = new List<PreferredStore>();

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
}
