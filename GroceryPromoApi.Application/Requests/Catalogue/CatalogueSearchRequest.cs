namespace GroceryPromoApi.Application.Requests.Catalogue;

public class CatalogueSearchRequest
{
    public string? Name { get; set; }

    public string? Category { get; set; }

    public Guid? SupermarketId { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
