using GroceryPromoApi.Application.Interfaces;
using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Interfaces.Services;
using GroceryPromoApi.Application.Models.PriceBarometer;
using GroceryPromoApi.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace GroceryPromoApi.Application.Services;

public class SyncService : ISyncService
{
    private readonly IPriceBarometerClient _priceBarometerClient;
    private readonly IBrochureRepository _brochureRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISupermarketRepository _supermarketRepository;
    private readonly ILogger<SyncService> _logger;

    public SyncService(
        IPriceBarometerClient priceBarometerClient,
        IBrochureRepository brochureRepository,
        IProductRepository productRepository,
        ISupermarketRepository supermarketRepository,
        ILogger<SyncService> logger)
    {
        _priceBarometerClient = priceBarometerClient;
        _brochureRepository = brochureRepository;
        _productRepository = productRepository;
        _supermarketRepository = supermarketRepository;
        _logger = logger;
    }

    public async Task SyncAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting sync...");

        var supermarkets = await _supermarketRepository.GetAllAsync(cancellationToken);
        var existingCodes = await _brochureRepository.GetAllBrochureCodesAsync(cancellationToken);

        foreach (var supermarket in supermarkets)
        {
            var rateLimitHit = await SyncSupermarketAsync(supermarket, existingCodes, cancellationToken);
            if (rateLimitHit)
            {
                _logger.LogWarning("Rate limit reached. Stopping sync early.");
                break;
            }
        }

        _logger.LogInformation("Sync complete.");
    }

    private async Task<bool> SyncSupermarketAsync(Supermarket supermarket, HashSet<string> existingCodes, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Syncing {Supermarket}...", supermarket.Name);

        var context = await PrepareBrochuresAsync(supermarket, existingCodes, cancellationToken);
        if (context is null)
            return false;

        return await FetchAndSaveProductsAsync(supermarket, context, cancellationToken);
    }

    private async Task<BrochureSyncContext?> PrepareBrochuresAsync(Supermarket supermarket, HashSet<string> existingCodes, CancellationToken cancellationToken)
    {
        var brochureResponses = await _priceBarometerClient.GetBrochuresAsync(supermarket.Slug, cancellationToken);
        var allApiCodes = brochureResponses.Select(b => b.BrochureCode).ToList();

        var inProgressBrochures = await _brochureRepository.GetInProgressBrochuresByCodesAsync(allApiCodes, supermarket.Id, cancellationToken);
        var inProgressCodes = inProgressBrochures.Select(b => b.BrochureCode).ToHashSet();

        var newBrochureResponses = brochureResponses
            .Where(b => !existingCodes.Contains(b.BrochureCode) && !inProgressCodes.Contains(b.BrochureCode))
            .ToList();

        if (newBrochureResponses.Count == 0 && inProgressBrochures.Count == 0)
        {
            _logger.LogInformation("No new or in-progress brochures for {Supermarket}.", supermarket.Name);
            return null;
        }

        _logger.LogInformation("Found {New} new and {InProgress} in-progress brochure(s) for {Supermarket}.",
            newBrochureResponses.Count, inProgressBrochures.Count, supermarket.Name);

        var activeBrochuresByCode = new Dictionary<string, Brochure>();

        foreach (var b in newBrochureResponses)
        {
            var brochure = new Brochure
            {
                Id = Guid.NewGuid(),
                BrochureCode = b.BrochureCode,
                ValidFrom = b.ValidFrom.HasValue ? DateTime.SpecifyKind(b.ValidFrom.Value, DateTimeKind.Utc) : null,
                ValidUntil = b.ValidUntil.HasValue ? DateTime.SpecifyKind(b.ValidUntil.Value, DateTimeKind.Utc) : null,
                SupermarketId = supermarket.Id,
                SyncedAt = DateTime.UtcNow,
                NextSyncPage = 1
            };

            await _brochureRepository.AddAsync(brochure, cancellationToken);
            activeBrochuresByCode[b.BrochureCode] = brochure;
            existingCodes.Add(b.BrochureCode);
        }

        foreach (var b in inProgressBrochures)
            activeBrochuresByCode[b.BrochureCode] = b;

        int startPage = newBrochureResponses.Count > 0
            ? 1
            : inProgressBrochures.Min(b => b.NextSyncPage!.Value);

        var seenExternalIds = new HashSet<int>();
        if (inProgressBrochures.Count > 0)
        {
            var inProgressIds = inProgressBrochures.Select(b => b.Id);
            seenExternalIds = await _productRepository.GetExternalIdsByBrochureIdsAsync(inProgressIds, cancellationToken);
            _logger.LogInformation("Loaded {Count} already-stored external IDs for in-progress brochures.", seenExternalIds.Count);
        }

        return new BrochureSyncContext(activeBrochuresByCode, startPage, seenExternalIds);
    }

    private async Task<bool> FetchAndSaveProductsAsync(Supermarket supermarket, BrochureSyncContext context, CancellationToken cancellationToken)
    {
        var (activeBrochuresByCode, startPage, seenExternalIds) = context;

        var products = new List<Product>();
        int page = startPage;
        int consecutivePagesWithNoNewProducts = 0;
        const int maxConsecutiveEmptyPages = 3;
        bool rateLimitHit = false;

        while (consecutivePagesWithNoNewProducts < maxConsecutiveEmptyPages)
        {
            List<ProductResponse> pageProducts;

            try
            {
                pageProducts = await _priceBarometerClient.GetProductsBySupermarketAsync(
                    supermarket.Slug, page, cancellationToken);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("401"))
            {
                _logger.LogWarning("Rate limit hit for {Supermarket} on page {Page}. Saving {Count} products collected so far.",
                    supermarket.Name, page, products.Count);
                rateLimitHit = true;
                break;
            }

            if (pageProducts.Count == 0)
                break;

            int newProductsOnPage = 0;

            foreach (var p in pageProducts)
            {
                if (string.IsNullOrWhiteSpace(p.Name))
                    continue;

                if (p.Brochure is null || !activeBrochuresByCode.TryGetValue(p.Brochure.Code, out var brochure))
                    continue;

                if (seenExternalIds.Contains(p.Id))
                    continue;

                seenExternalIds.Add(p.Id);
                newProductsOnPage++;

                products.Add(new Product
                {
                    Id = Guid.NewGuid(),
                    ExternalId = p.Id,
                    Name = p.Name,
                    NormalizedName = p.Name.ToLower().Trim(),
                    Description = p.Description,
                    PriceLev = p.PriceLev,
                    OldPriceLev = p.OldPriceLev,
                    PriceEur = p.PriceEur,
                    OldPriceEur = p.OldPriceEur,
                    Discount = p.Discount,
                    Quantity = p.Quantity,
                    NormalizedQuantity = p.Quantity?.ToLower().Trim(),
                    Category = p.Category,
                    ImageUrl = p.ImageUrl,
                    ValidFrom = brochure.ValidFrom,
                    ValidUntil = brochure.ValidUntil,
                    BrochureId = brochure.Id,
                    SupermarketId = supermarket.Id
                });
            }

            if (newProductsOnPage == 0)
                consecutivePagesWithNoNewProducts++;
            else
                consecutivePagesWithNoNewProducts = 0;

            page++;
        }

        if (products.Count > 0)
            await _productRepository.AddRangeAsync(products, cancellationToken);

        foreach (var brochure in activeBrochuresByCode.Values)
        {
            brochure.NextSyncPage = rateLimitHit ? page : null;
            await _brochureRepository.UpdateAsync(brochure, cancellationToken);
        }

        _logger.LogInformation("Synced {Supermarket}: {Count} products. {Status}",
            supermarket.Name, products.Count,
            rateLimitHit ? $"Interrupted at page {page} — will resume tomorrow." : "Fully synced.");

        return rateLimitHit;
    }
}
