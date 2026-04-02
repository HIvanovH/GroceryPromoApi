using GroceryPromoApi.Application.DTOs.Products;
using GroceryPromoApi.Application.Interfaces.Services;
using GroceryPromoApi.Application.Requests.Products;
using Microsoft.AspNetCore.Mvc;

namespace GroceryPromoApi.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] ProductSearchRequest request, CancellationToken cancellationToken)
    {
        var result = await _productService.SearchAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _productService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }
}
