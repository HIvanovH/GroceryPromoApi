using GroceryPromoApi.Application.DTOs.Auth;
using GroceryPromoApi.Application.Requests.Auth;

namespace GroceryPromoApi.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    
    Task<AuthResponse> RefreshTokenAsync(RefreshRequest request, CancellationToken cancellationToken = default);
    
    Task LogoutAsync(Guid sessionId, CancellationToken cancellationToken = default);
    
    Task UpdateFcmTokenAsync(Guid sessionId, string fcmToken, CancellationToken cancellationToken = default);
}
