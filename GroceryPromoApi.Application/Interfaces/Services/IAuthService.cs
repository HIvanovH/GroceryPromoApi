namespace GroceryPromoApi.Application.Interfaces.Services;

public interface IAuthService
{
    Task RegisterAsync();
    Task LoginAsync();
    Task RefreshTokenAsync();
    Task LogoutAsync();
    Task GoogleLoginAsync();
    Task UpdateFcmTokenAsync();
}
