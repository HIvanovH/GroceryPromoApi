using AutoMapper;
using GroceryPromoApi.Application.DTOs.Catalogue;
using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Interfaces.Services;
using GroceryPromoApi.Application.Requests.Catalogue;

namespace GroceryPromoApi.Application.Services;

public class CatalogueProductService : ICatalogueProductService
{
    private readonly ICatalogueProductRepository _catalogueProductRepository;
    private readonly IMapper _mapper;

    public CatalogueProductService(ICatalogueProductRepository catalogueProductRepository, IMapper mapper)
    {
        _catalogueProductRepository = catalogueProductRepository;
        _mapper = mapper;
    }

    public async Task<List<CatalogueProductDTO>> SearchAsync(CatalogueSearchRequest request, CancellationToken cancellationToken = default)
    {
        var products = await _catalogueProductRepository.SearchAsync(
            request.Name, request.Category, request.SupermarketId,
            request.Page, request.PageSize, cancellationToken);

        return _mapper.Map<List<CatalogueProductDTO>>(products);
    }
}
