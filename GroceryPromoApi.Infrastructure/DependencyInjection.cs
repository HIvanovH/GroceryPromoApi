using GroceryPromoApi.Application.Interfaces;
using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Options;
using GroceryPromoApi.Infrastructure.Database.Repositories;
using GroceryPromoApi.Infrastructure.DbContext;
using GroceryPromoApi.Infrastructure.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryPromoApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("grocery-db")));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddOptions<PriceBarometerOptions>()
            .BindConfiguration(PriceBarometerOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

#pragma warning disable EXTEXP0001
        services.AddHttpClient<IPriceBarometerClient, PriceBarometerClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PriceBarometerOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", options.ApiKey);
        })
        .RemoveAllResilienceHandlers()
        .AddStandardResilienceHandler(options =>
        {
            options.Retry.MaxRetryAttempts = 1;
        });
#pragma warning restore EXTEXP0001

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();
        services.AddScoped<ISupermarketRepository, SupermarketRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
services.AddScoped<IPreferredStoreRepository, PreferredStoreRepository>();
        services.AddScoped<IBrochureRepository, BrochureRepository>();
        services.AddScoped<ICatalogueProductRepository, CatalogueProductRepository>();

        return services;
    }
}
