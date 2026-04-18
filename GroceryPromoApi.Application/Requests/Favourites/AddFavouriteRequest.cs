using System.ComponentModel.DataAnnotations;

namespace GroceryPromoApi.Application.Requests.Favourites;

public class AddFavouriteRequest
{
    [Required]
    public Guid CatalogueProductId { get; set; }
}
