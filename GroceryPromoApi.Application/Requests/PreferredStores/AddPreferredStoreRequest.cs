using System.ComponentModel.DataAnnotations;

namespace GroceryPromoApi.Application.Requests.PreferredStores;

public class AddPreferredStoreRequest
{
    [Required]
    public Guid SupermarketId { get; set; }
}
