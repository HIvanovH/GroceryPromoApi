using System.Text.Json.Serialization;

namespace GroceryPromoApi.Application.Models.PriceBarometer;

public class ProductResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("price_lev")]
    public decimal PriceLev { get; set; }

    [JsonPropertyName("old_price_lev")]
    public decimal? OldPriceLev { get; set; }

    [JsonPropertyName("price_eur")]
    public decimal PriceEur { get; set; }

    [JsonPropertyName("old_price_eur")]
    public decimal? OldPriceEur { get; set; }

    [JsonPropertyName("discount")]
    public string? Discount { get; set; }

    [JsonPropertyName("quantity")]
    public string? Quantity { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("brochure")]
    public BrochureInfo Brochure { get; set; } = null!;

    [JsonPropertyName("supermarket")]
    public SupermarketInfo Supermarket { get; set; } = null!;
}
