using System.ComponentModel.DataAnnotations;

namespace GroceryPromoApi.Application.Requests.Favourites
{
    public class AddFavouriteRequest
    {
        [Required]
        public string? NormalizedName { get; set; }

        public string? NormalizedQuantity { get; set; }

        public string? Category { get; set; }
    }
}
