using GroceryPromoApi.Application.Interfaces.Services;
using GroceryPromoApi.Application.Requests.Catalogue;
using Microsoft.AspNetCore.Mvc;

namespace GroceryPromoApi.Controllers
{
    [ApiController]
    [Route("api/v1/catalogue")]
    public class CatalogueController : ControllerBase
    {
        private readonly ICatalogueProductService _catalogueProductService;

        public CatalogueController(ICatalogueProductService catalogueProductService)
        {
            _catalogueProductService = catalogueProductService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] CatalogueSearchRequest request, CancellationToken cancellationToken)
        {
            var result = await _catalogueProductService.SearchAsync(request, cancellationToken);
            return Ok(result);
        }
    }
}