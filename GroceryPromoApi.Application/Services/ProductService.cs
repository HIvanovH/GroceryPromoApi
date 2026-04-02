using AutoMapper;
using GroceryPromoApi.Application.DTOs.Products;
using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Interfaces.Services;
using GroceryPromoApi.Application.Requests.Products;
using GroceryPromoApi.Domain.Exceptions;

namespace GroceryPromoApi.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<List<ProductDTO>> SearchAsync(ProductSearchRequest request, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.SearchAsync(
            request.Name, request.SupermarketId, request.Category,
            request.Page, request.PageSize, cancellationToken);

        return _mapper.Map<List<ProductDTO>>(products);
    }

    public async Task<ProductDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product {id} not found.");

        return _mapper.Map<ProductDTO>(product);
    }
}
