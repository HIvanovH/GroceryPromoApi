using GroceryPromoApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryPromoApi.Application.DTOs.Favorites
{
    public class FavouriteProductDTO
    {
        public Guid Id { get; set; }

        public string Type { get; set; } = "Product";

        public string? NormalizedName { get; set; }

        public string? NormalizedQuantity { get; set; }

        public string? Category { get; set; }
    }
}
