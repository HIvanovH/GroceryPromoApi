using GroceryPromoApi.Application.Interfaces;
using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Models.PriceBarometer;
using GroceryPromoApi.Application.Services;
using GroceryPromoApi.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace GroceryPromoApi.Tests.Sync;

public class SyncServiceTests
{
    private readonly Mock<IPriceBarometerClient> _client = new();
    private readonly Mock<IBrochureRepository> _brochureRepo = new();
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<ISupermarketRepository> _supermarketRepo = new();
    private readonly Mock<ICatalogueProductRepository> _catalogueRepo = new();
    private readonly SyncService _syncService;

    public SyncServiceTests()
    {
        _syncService = new SyncService(
            _client.Object,
            _brochureRepo.Object,
            _productRepo.Object,
            _supermarketRepo.Object,
            _catalogueRepo.Object,
            Mock.Of<ILogger<SyncService>>());
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private static Supermarket MakeSupermarket(string name = "Billa") => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Slug = name.ToLower()
    };

    private static BrochureResponse MakeBrochureResponse(string code = "BR001") => new()
    {
        BrochureCode = code,
        ValidFrom = DateTime.UtcNow.AddDays(-1),
        ValidUntil = DateTime.UtcNow.AddDays(7)
    };

    private static ProductResponse MakeProduct(string brochureCode = "BR001", string name = "Milk", int id = 1) => new()
    {
        Id = id,
        Name = name,
        PriceEur = 1.50m,
        OldPriceEur = 2.00m,
        Quantity = "1l",
        Category = "Dairy",
        Brochure = new BrochureInfo { Code = brochureCode }
    };

    private void SetupBasicBrochureRepo(HashSet<string>? existingCodes = null)
    {
        _brochureRepo.Setup(r => r.GetAllBrochureCodesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCodes ?? new HashSet<string>());
        _brochureRepo.Setup(r => r.GetInProgressBrochuresByCodesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Brochure>());
        _brochureRepo.Setup(r => r.AddAsync(It.IsAny<Brochure>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _brochureRepo.Setup(r => r.UpdateAsync(It.IsAny<Brochure>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    private void SetupBasicProductRepo()
    {
        _productRepo.Setup(r => r.GetExternalIdsByBrochureIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<int>());
        _productRepo.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    private void SetupBasicCatalogueRepo()
    {
        _catalogueRepo.Setup(r => r.GetByIdentityAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CatalogueProduct?)null);
        _catalogueRepo.Setup(r => r.AddAsync(It.IsAny<CatalogueProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _catalogueRepo.Setup(r => r.UpdateAsync(It.IsAny<CatalogueProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _catalogueRepo.Setup(r => r.UpsertOfferAsync(It.IsAny<CatalogueProductOffer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    // ── Happy path ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Sync_NewProduct_CreatesCatalogueProductAndOffer()
    {
        var supermarket = MakeSupermarket();
        _supermarketRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Supermarket> { supermarket });

        SetupBasicBrochureRepo();
        SetupBasicProductRepo();
        SetupBasicCatalogueRepo();

        _client.Setup(c => c.GetBrochuresAsync(supermarket.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BrochureResponse> { MakeBrochureResponse() });

        _client.SetupSequence(c => c.GetProductsBySupermarketAsync(supermarket.Slug, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductResponse> { MakeProduct() })
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>());

        await _syncService.SyncAsync();

        _catalogueRepo.Verify(r => r.AddAsync(It.IsAny<CatalogueProduct>(), It.IsAny<CancellationToken>()), Times.Once);
        _catalogueRepo.Verify(r => r.UpsertOfferAsync(It.IsAny<CatalogueProductOffer>(), It.IsAny<CancellationToken>()), Times.Once);
        _productRepo.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── Deduplication ──────────────────────────────────────────────────────

    [Fact]
    public async Task Sync_SameProductTwoSupermarkets_OneCatalogueProductTwoOffers()
    {
        var billa = MakeSupermarket("Billa");
        var kaufland = MakeSupermarket("Kaufland");

        _supermarketRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Supermarket> { billa, kaufland });

        SetupBasicBrochureRepo();
        SetupBasicProductRepo();

        var existingCatalogueProduct = new CatalogueProduct { Id = Guid.NewGuid(), Name = "Milk", NormalizedName = "milk", NormalizedQuantity = "1l" };

        _catalogueRepo.SetupSequence(r => r.GetByIdentityAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CatalogueProduct?)null)       // first supermarket — not found, create
            .ReturnsAsync(existingCatalogueProduct);     // second supermarket — found, update
        _catalogueRepo.Setup(r => r.AddAsync(It.IsAny<CatalogueProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _catalogueRepo.Setup(r => r.UpdateAsync(It.IsAny<CatalogueProduct>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _catalogueRepo.Setup(r => r.UpsertOfferAsync(It.IsAny<CatalogueProductOffer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _client.Setup(c => c.GetBrochuresAsync(billa.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BrochureResponse> { MakeBrochureResponse("BR-BILLA") });
        _client.Setup(c => c.GetBrochuresAsync(kaufland.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BrochureResponse> { MakeBrochureResponse("BR-KAUFLAND") });

        _client.SetupSequence(c => c.GetProductsBySupermarketAsync(billa.Slug, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductResponse> { MakeProduct("BR-BILLA", "Milk", 1) })
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>());

        _client.SetupSequence(c => c.GetProductsBySupermarketAsync(kaufland.Slug, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductResponse> { MakeProduct("BR-KAUFLAND", "Milk", 2) })
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>());

        await _syncService.SyncAsync();

        _catalogueRepo.Verify(r => r.AddAsync(It.IsAny<CatalogueProduct>(), It.IsAny<CancellationToken>()), Times.Once);
        _catalogueRepo.Verify(r => r.UpdateAsync(It.IsAny<CatalogueProduct>(), It.IsAny<CancellationToken>()), Times.Once);
        _catalogueRepo.Verify(r => r.UpsertOfferAsync(It.IsAny<CatalogueProductOffer>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    // ── Skip rules ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Sync_ProductWithEmptyName_IsSkipped()
    {
        var supermarket = MakeSupermarket();
        _supermarketRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Supermarket> { supermarket });

        SetupBasicBrochureRepo();
        SetupBasicProductRepo();
        SetupBasicCatalogueRepo();

        _client.Setup(c => c.GetBrochuresAsync(supermarket.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BrochureResponse> { MakeBrochureResponse() });

        _client.SetupSequence(c => c.GetProductsBySupermarketAsync(supermarket.Slug, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductResponse> { MakeProduct(name: "") })
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>());

        await _syncService.SyncAsync();

        _catalogueRepo.Verify(r => r.AddAsync(It.IsAny<CatalogueProduct>(), It.IsAny<CancellationToken>()), Times.Never);
        _productRepo.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Sync_ProductWithUnknownBrochureCode_IsSkipped()
    {
        var supermarket = MakeSupermarket();
        _supermarketRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Supermarket> { supermarket });

        SetupBasicBrochureRepo();
        SetupBasicProductRepo();
        SetupBasicCatalogueRepo();

        _client.Setup(c => c.GetBrochuresAsync(supermarket.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BrochureResponse> { MakeBrochureResponse("KNOWN-CODE") });

        _client.SetupSequence(c => c.GetProductsBySupermarketAsync(supermarket.Slug, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductResponse> { MakeProduct(brochureCode: "UNKNOWN-CODE") })
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>());

        await _syncService.SyncAsync();

        _catalogueRepo.Verify(r => r.AddAsync(It.IsAny<CatalogueProduct>(), It.IsAny<CancellationToken>()), Times.Never);
        _productRepo.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Sync_AlreadySeenExternalId_IsSkipped()
    {
        var supermarket = MakeSupermarket();
        _supermarketRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Supermarket> { supermarket });

        SetupBasicBrochureRepo();
        SetupBasicCatalogueRepo();

        var inProgressBrochure = new Brochure { Id = Guid.NewGuid(), BrochureCode = "BR001", NextSyncPage = 2, SupermarketId = supermarket.Id };
        _brochureRepo.Setup(r => r.GetInProgressBrochuresByCodesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Brochure> { inProgressBrochure });
        _productRepo.Setup(r => r.GetExternalIdsByBrochureIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<int> { 1 }); // ExternalId 1 already seen
        _productRepo.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _client.Setup(c => c.GetBrochuresAsync(supermarket.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BrochureResponse> { MakeBrochureResponse("BR001") });

        _client.SetupSequence(c => c.GetProductsBySupermarketAsync(supermarket.Slug, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductResponse> { MakeProduct(id: 1) }) // id 1 — already seen
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>());

        await _syncService.SyncAsync();

        _catalogueRepo.Verify(r => r.AddAsync(It.IsAny<CatalogueProduct>(), It.IsAny<CancellationToken>()), Times.Never);
        _productRepo.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ── Pagination ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Sync_StopsAfter3ConsecutiveEmptyPages()
    {
        var supermarket = MakeSupermarket();
        _supermarketRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Supermarket> { supermarket });

        SetupBasicBrochureRepo();
        SetupBasicProductRepo();
        SetupBasicCatalogueRepo();

        _client.Setup(c => c.GetBrochuresAsync(supermarket.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BrochureResponse> { MakeBrochureResponse() });

        // pages 2-4 return products but with empty names → all filtered → consecutivePagesWithNoNewProducts increments
        var skippedProduct = MakeProduct(name: "");
        _client.SetupSequence(c => c.GetProductsBySupermarketAsync(supermarket.Slug, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductResponse> { MakeProduct() })          // page 1: 1 new product
            .ReturnsAsync(new List<ProductResponse> { skippedProduct })          // page 2: all filtered (1)
            .ReturnsAsync(new List<ProductResponse> { skippedProduct })          // page 3: all filtered (2)
            .ReturnsAsync(new List<ProductResponse> { skippedProduct });         // page 4: all filtered (3) → stop

        await _syncService.SyncAsync();

        _client.Verify(c => c.GetProductsBySupermarketAsync(supermarket.Slug, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
    }

    // ── Rate limit handling ────────────────────────────────────────────────

    [Fact]
    public async Task Sync_RateLimitDuringBrochureFetch_StopsSyncEarly()
    {
        var billa = MakeSupermarket("Billa");
        var kaufland = MakeSupermarket("Kaufland");

        _supermarketRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Supermarket> { billa, kaufland });

        _brochureRepo.Setup(r => r.GetAllBrochureCodesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>());

        _client.Setup(c => c.GetBrochuresAsync(billa.Slug, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("401 Unauthorized"));

        await _syncService.SyncAsync();

        // Kaufland should never be reached
        _client.Verify(c => c.GetBrochuresAsync(kaufland.Slug, It.IsAny<CancellationToken>()), Times.Never);
        _productRepo.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Sync_RateLimitDuringProductFetch_SavesCollectedAndSetsNextSyncPage()
    {
        var supermarket = MakeSupermarket();
        _supermarketRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Supermarket> { supermarket });

        SetupBasicBrochureRepo();
        SetupBasicProductRepo();
        SetupBasicCatalogueRepo();

        _client.Setup(c => c.GetBrochuresAsync(supermarket.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BrochureResponse> { MakeBrochureResponse() });

        _client.SetupSequence(c => c.GetProductsBySupermarketAsync(supermarket.Slug, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductResponse> { MakeProduct(id: 1), MakeProduct(id: 2, name: "Butter") })
            .ThrowsAsync(new InvalidOperationException("401 Unauthorized"));

        await _syncService.SyncAsync();

        _productRepo.Verify(r => r.AddRangeAsync(
            It.Is<IEnumerable<Product>>(p => p.Count() == 2),
            It.IsAny<CancellationToken>()), Times.Once);

        _brochureRepo.Verify(r => r.UpdateAsync(
            It.Is<Brochure>(b => b.NextSyncPage == 2),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Sync_HttpErrorDuringBrochureFetch_SkipsSupermarketContinues()
    {
        var billa = MakeSupermarket("Billa");
        var kaufland = MakeSupermarket("Kaufland");

        _supermarketRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Supermarket> { billa, kaufland });

        _brochureRepo.Setup(r => r.GetAllBrochureCodesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>());
        _brochureRepo.Setup(r => r.GetInProgressBrochuresByCodesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Brochure>());
        _brochureRepo.Setup(r => r.AddAsync(It.IsAny<Brochure>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _brochureRepo.Setup(r => r.UpdateAsync(It.IsAny<Brochure>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        SetupBasicProductRepo();
        SetupBasicCatalogueRepo();

        _client.Setup(c => c.GetBrochuresAsync(billa.Slug, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        _client.Setup(c => c.GetBrochuresAsync(kaufland.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BrochureResponse> { MakeBrochureResponse("BR-KAUFLAND") });

        _client.SetupSequence(c => c.GetProductsBySupermarketAsync(kaufland.Slug, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductResponse> { MakeProduct("BR-KAUFLAND") })
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>());

        await _syncService.SyncAsync();

        _productRepo.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── In-progress resume ─────────────────────────────────────────────────

    [Fact]
    public async Task Sync_InProgressBrochure_ResumesFromNextSyncPage()
    {
        var supermarket = MakeSupermarket();
        _supermarketRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Supermarket> { supermarket });

        _brochureRepo.Setup(r => r.GetAllBrochureCodesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string> { "BR001" }); // already known
        _brochureRepo.Setup(r => r.UpdateAsync(It.IsAny<Brochure>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var inProgressBrochure = new Brochure { Id = Guid.NewGuid(), BrochureCode = "BR001", NextSyncPage = 3, SupermarketId = supermarket.Id };
        _brochureRepo.Setup(r => r.GetInProgressBrochuresByCodesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Brochure> { inProgressBrochure });

        _productRepo.Setup(r => r.GetExternalIdsByBrochureIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<int>());
        _productRepo.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        SetupBasicCatalogueRepo();

        _client.Setup(c => c.GetBrochuresAsync(supermarket.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BrochureResponse> { MakeBrochureResponse("BR001") });

        _client.SetupSequence(c => c.GetProductsBySupermarketAsync(supermarket.Slug, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>())
            .ReturnsAsync(new List<ProductResponse>());

        await _syncService.SyncAsync();

        _client.Verify(c => c.GetProductsBySupermarketAsync(supermarket.Slug, 3, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── No work to do ──────────────────────────────────────────────────────

    [Fact]
    public async Task Sync_NoNewOrInProgressBrochures_DoesNotFetchProducts()
    {
        var supermarket = MakeSupermarket();
        _supermarketRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Supermarket> { supermarket });

        _brochureRepo.Setup(r => r.GetAllBrochureCodesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string> { "BR001" }); // already synced
        _brochureRepo.Setup(r => r.GetInProgressBrochuresByCodesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Brochure>()); // nothing in progress

        _client.Setup(c => c.GetBrochuresAsync(supermarket.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BrochureResponse> { MakeBrochureResponse("BR001") });

        await _syncService.SyncAsync();

        _client.Verify(c => c.GetProductsBySupermarketAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
