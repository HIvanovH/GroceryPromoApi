using System.ComponentModel.DataAnnotations;

namespace GroceryPromoApi.Application.Requests.Products;

public class ProductSearchRequest
{
    public string? Name { get; set; }

    public Guid? SupermarketId { get; set; }

    public string? Category { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}
