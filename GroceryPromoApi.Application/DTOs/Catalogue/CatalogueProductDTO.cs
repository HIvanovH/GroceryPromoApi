namespace GroceryPromoApi.Application.DTOs.Catalogue;

public class CatalogueProductDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Quantity { get; set; }

    public string? Category { get; set; }

    public string? ImageUrl { get; set; }

    public List<CatalogueProductOfferDTO> Offers { get; set; } = new();
}
