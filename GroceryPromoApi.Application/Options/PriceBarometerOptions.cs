using System.ComponentModel.DataAnnotations;

namespace GroceryPromoApi.Application.Options;

public class PriceBarometerOptions
{
    public const string SectionName = "PriceBarometer";

    [Required]
    public string BaseUrl { get; set; } = string.Empty;

    [Required]
    public string ApiKey { get; set; } = string.Empty;
}
