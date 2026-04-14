using AutoMapper;
using GroceryPromoApi.Application.DTOs.Supermarkets;
using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Interfaces.Services;

namespace GroceryPromoApi.Application.Services;

public class SupermarketService : ISupermarketService
{
    private readonly ISupermarketRepository _supermarketRepository;
    private readonly IMapper _mapper;

    public SupermarketService(ISupermarketRepository supermarketRepository, IMapper mapper)
    {
        _supermarketRepository = supermarketRepository;
        _mapper = mapper;
    }

    public async Task<List<SupermarketDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var supermarkets = await _supermarketRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<SupermarketDTO>>(supermarkets);
    }
}
