using GroceryPromoApi.Application.DTOs.Favorites;
using GroceryPromoApi.Application.Interfaces.Services;
using GroceryPromoApi.Application.Requests.Favourites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GroceryPromoApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/favourites")]
    public class FavouritesController : ControllerBase
    {
        private readonly IFavouriteService _favouriteService;

        public FavouritesController(IFavouriteService favouriteService)
        {
            _favouriteService = favouriteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavouritesByUserId(CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();

            var result = await _favouriteService.GetFavouritesAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavouriteAsync([FromBody] AddFavouriteRequest request, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();

            var result = await _favouriteService.AddFavouriteAsync(userId, request, cancellationToken);
            return CreatedAtAction(nameof(GetFavouritesByUserId), null, result);
        }

        [HttpDelete("{favouriteId:guid}")]
        public async Task<IActionResult> RemoveFavouriteAsync([FromRoute] Guid favouriteId, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized();

            await _favouriteService.RemoveFavouriteAsync(userId, favouriteId, cancellationToken);
            return NoContent();
        }
    }
}
