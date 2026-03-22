namespace GroceryPromoApi.Domain.Entities;

public class Supermarket
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public ICollection<Brochure> Brochures { get; set; } = new List<Brochure>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<PreferredStore> PreferredStores { get; set; } = new List<PreferredStore>();
}
