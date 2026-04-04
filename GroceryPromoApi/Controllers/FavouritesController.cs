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
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _favouriteService.GetFavouritesAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavouriteAsync([FromBody] AddFavouriteRequest request, CancellationToken cancellationToken = default)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _favouriteService.AddFavouriteAsync(userId, request, cancellationToken);
            return CreatedAtAction(nameof(GetFavouritesByUserId), result);
        }

        [HttpDelete("{favouriteId:guid}")]
        public async Task<IActionResult> RemoveFavouriteAsync([FromRoute] Guid favouriteId, CancellationToken cancellationToken = default)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _favouriteService.RemoveFavouriteAsync(userId, favouriteId, cancellationToken);
            return NoContent();
        }
    }
}
