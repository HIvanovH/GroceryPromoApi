using GroceryPromoApi.Application.Interfaces.Services;
using GroceryPromoApi.Application.Options;
using GroceryPromoApi.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryPromoApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IFavouriteService, FavouriteService>();
        services.AddScoped<IPreferredStoreService, PreferredStoreService>();
        services.AddScoped<ISyncService, SyncService>();
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
