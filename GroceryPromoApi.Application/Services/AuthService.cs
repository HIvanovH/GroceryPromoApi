using GroceryPromoApi.Application.Interfaces.Services;

namespace GroceryPromoApi.Application.Services;

public class AuthService : IAuthService
{
    public Task RegisterAsync() => throw new NotImplementedException();
    public Task LoginAsync() => throw new NotImplementedException();
    public Task RefreshTokenAsync() => throw new NotImplementedException();
    public Task LogoutAsync() => throw new NotImplementedException();
    public Task GoogleLoginAsync() => throw new NotImplementedException();
    public Task UpdateFcmTokenAsync() => throw new NotImplementedException();
}
