namespace GroceryPromoApi.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool Success { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
