using GroceryPromoApi.Application.Interfaces;
using GroceryPromoApi.Application.Models.PriceBarometer;
using System.Net;
using System.Net.Http.Json;

namespace GroceryPromoApi.Infrastructure.Http;

public class PriceBarometerClient : IPriceBarometerClient
{
    private readonly HttpClient _httpClient;

    public PriceBarometerClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<BrochureResponse>> GetBrochuresAsync(string? supermarketSlug = null, CancellationToken cancellationToken = default)
    {
        var url = supermarketSlug is not null
            ? $"brochures?supermarket={supermarketSlug}"
            : "brochures";

        return await GetPagedAsync<BrochureResponse>(url, cancellationToken);
    }

    public async Task<List<ProductResponse>> GetProductsBySupermarketAsync(string supermarketSlug, int page, CancellationToken cancellationToken = default)
    {
        return await GetPagedAsync<ProductResponse>(
            $"products?supermarket={supermarketSlug}&only_valid=true&page={page}", cancellationToken);
    }

    private async Task<List<T>> GetPagedAsync<T>(string url, CancellationToken cancellationToken)
    {
        var httpResponse = await _httpClient.GetAsync(url, cancellationToken);

        if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
            throw new InvalidOperationException("Price Barometer API returned 401 — daily request limit reached or API key is invalid.");

        httpResponse.EnsureSuccessStatusCode();

        var response = await httpResponse.Content.ReadFromJsonAsync<PagedApiResponse<T>>(cancellationToken);
        return response?.Data ?? new List<T>();
    }
}
