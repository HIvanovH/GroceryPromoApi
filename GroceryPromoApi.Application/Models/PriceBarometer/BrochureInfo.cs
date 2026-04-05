using System.Text.Json.Serialization;

namespace GroceryPromoApi.Application.Models.PriceBarometer;

public class BrochureInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("valid_from")]
    public DateTime? ValidFrom { get; set; }

    [JsonPropertyName("valid_until")]
    public DateTime? ValidUntil { get; set; }
}
