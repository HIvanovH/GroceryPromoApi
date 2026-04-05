using System.Text.Json.Serialization;

namespace GroceryPromoApi.Application.Models.PriceBarometer;

public class SupermarketInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;
}
