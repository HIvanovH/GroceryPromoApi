using System.Text.Json.Serialization;

namespace GroceryPromoApi.Application.Models.PriceBarometer;

public class PagedApiResponse<T>
{
    [JsonPropertyName("data")]
    public List<T> Data { get; set; } = new();
}
