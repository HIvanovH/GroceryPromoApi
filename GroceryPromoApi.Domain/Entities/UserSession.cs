namespace GroceryPromoApi.Domain.Entities;

public class UserSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string RefreshToken { get; set; } = string.Empty;        
    public string? PreviousRefreshToken { get; set; }              
    public string? FcmToken { get; set; }                          

    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime LastUsedAt { get; set; }
}
