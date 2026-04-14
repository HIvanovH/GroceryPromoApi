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
    public void Normalize_CyrillicL_WithDecimalComma_ReturnsLatinL()
    {
        Assert.Equal("1.5l", QuantityNormalizer.Normalize("1,5 л"));
    }

    [Fact]
    public void Normalize_MultipackWithCyrillicL_ReturnsLatinMultipack()
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
        Assert.Equal("1kg", QuantityNormalizer.Normalize("1кг /опаковка"));
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
}
