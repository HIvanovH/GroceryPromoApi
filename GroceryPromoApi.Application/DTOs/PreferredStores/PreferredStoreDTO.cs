namespace GroceryPromoApi.Application.DTOs.PreferredStores;

public class PreferredStoreDTO
{
    public Guid Id { get; set; }
    public Guid SupermarketId { get; set; }
    public string SupermarketName { get; set; } = string.Empty;
    public string SupermarketSlug { get; set; } = string.Empty;
}
