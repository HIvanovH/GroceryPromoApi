namespace GroceryPromoApi.Application.Interfaces.Services;

public interface INotificationService
{
    Task SendWeeklyDigestAsync();
}
