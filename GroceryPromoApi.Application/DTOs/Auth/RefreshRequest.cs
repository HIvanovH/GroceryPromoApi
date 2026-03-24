using System.ComponentModel.DataAnnotations;

namespace GroceryPromoApi.Application.DTOs.Auth;

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;

}
