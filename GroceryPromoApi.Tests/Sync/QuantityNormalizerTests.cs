using GroceryPromoApi.Application.Services;

namespace GroceryPromoApi.Tests.Sync;

public class QuantityNormalizerTests
{
    [Fact]
    public void Normalize_Null_ReturnsNull()
    {
        Assert.Null(QuantityNormalizer.Normalize(null));
    }

    [Fact]
    public void Normalize_Whitespace_ReturnsNull()
    {
        Assert.Null(QuantityNormalizer.Normalize("   "));
    }

    [Fact]
    public void Normalize_CyrillicMl_ReturnsLatinMl()
    {
        Assert.Equal("250ml", QuantityNormalizer.Normalize("250 мл"));
    }

    [Fact]
    public void Normalize_CyrillicL_WithDecimalComma_ConvertsToMl()
    {
        Assert.Equal("1500ml", QuantityNormalizer.Normalize("1,5 л"));
    }

    [Fact]
    public void Normalize_MultipackWithCyrillicL_SkipsConversion()
    {
        Assert.Equal("4x0.5l", QuantityNormalizer.Normalize("4 x 0.5 л"));
    }

    [Fact]
    public void Normalize_CyrillicGrWithDot_ReturnsG()
    {
        Assert.Equal("500g", QuantityNormalizer.Normalize("500гр."));
    }

    [Fact]
    public void Normalize_CyrillicBrWithDot_ReturnsBr()
    {
        Assert.Equal("2br", QuantityNormalizer.Normalize("2 бр."));
    }

    [Fact]
    public void Normalize_WithPackagingSuffix_StripsIt()
    {
        Assert.Equal("1000g", QuantityNormalizer.Normalize("1кг /опаковка"));
    }

    [Fact]
    public void Normalize_WithApproximatePrefix_StripsIt()
    {
        Assert.Equal("250g", QuantityNormalizer.Normalize("~250g"));
    }

    [Fact]
    public void Normalize_WithTrailingDot_StripsIt()
    {
        Assert.Equal("250g", QuantityNormalizer.Normalize("250 g."));
    }

    // ── Unit conversion to base units ──────────────────────────────────────

    [Fact]
    public void Normalize_Kg_ConvertsToG()
    {
        Assert.Equal("500g", QuantityNormalizer.Normalize("0.5kg"));
    }

    [Fact]
    public void Normalize_CyrillicKg_ConvertsToG()
    {
        Assert.Equal("500g", QuantityNormalizer.Normalize("0,5кг"));
    }

    [Fact]
    public void Normalize_WholeKg_ConvertsToG()
    {
        Assert.Equal("1000g", QuantityNormalizer.Normalize("1kg"));
    }

    [Fact]
    public void Normalize_L_ConvertsToMl()
    {
        Assert.Equal("500ml", QuantityNormalizer.Normalize("0.5l"));
    }

    [Fact]
    public void Normalize_500g_StaysAsG()
    {
        Assert.Equal("500g", QuantityNormalizer.Normalize("500g"));
    }

    [Fact]
    public void Normalize_500ml_StaysAsMl()
    {
        Assert.Equal("500ml", QuantityNormalizer.Normalize("500мл"));
    }
}
