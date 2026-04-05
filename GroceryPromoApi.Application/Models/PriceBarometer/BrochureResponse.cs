using System.Text.Json.Serialization;

namespace GroceryPromoApi.Application.Models.PriceBarometer;

public class BrochureResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("brochure_code")]
    public string BrochureCode { get; set; } = string.Empty;

    [JsonPropertyName("valid_from")]
    public DateTime? ValidFrom { get; set; }

    [JsonPropertyName("valid_until")]
    public DateTime? ValidUntil { get; set; }

    [JsonPropertyName("supermarket")]
    public SupermarketInfo Supermarket { get; set; } = null!;

    [JsonPropertyName("pages_count")]
    public int PagesCount { get; set; }
}
