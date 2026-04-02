namespace GroceryPromoApi.Application.Requests.Products;

public class ProductSearchRequest
{
    public string? Name { get; set; }
    public Guid? SupermarketId { get; set; }
    public string? Category { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
