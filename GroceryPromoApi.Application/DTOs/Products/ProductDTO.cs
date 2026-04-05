namespace GroceryPromoApi.Application.DTOs.Products
{
    public class ProductDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal PriceLev { get; set; }

        public decimal? OldPriceLev { get; set; }

        public decimal PriceEur { get; set; }

        public decimal? OldPriceEur { get; set; }

        public string? Discount { get; set; }

        public string? Quantity { get; set; }

        public string? Category { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidUntil { get; set; }

        public Guid SupermarketId { get; set; }

        public string SupermarketName { get; set; } = string.Empty;
    }
}