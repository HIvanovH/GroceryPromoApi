using GroceryPromoApi.Application.Interfaces.Services;
using GroceryPromoApi.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryPromoApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
