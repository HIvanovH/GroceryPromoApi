using AutoMapper;
using GroceryPromoApi.Application.DTOs.Catalogue;
using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Interfaces.Services;
using GroceryPromoApi.Application.Requests.Favourites;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Domain.Exceptions;
using System.Linq;

namespace GroceryPromoApi.Application.Services;

public class FavouriteService : IFavouriteService
{
    private readonly IUserRepository _userRepository;
    private readonly ICatalogueProductRepository _catalogueProductRepository;
    private readonly IMapper _mapper;

    public FavouriteService(IUserRepository userRepository, ICatalogueProductRepository catalogueProductRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _catalogueProductRepository = catalogueProductRepository;
        _mapper = mapper;
    }

    public async Task<List<CatalogueProductDTO>> GetFavouritesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetWithFavouritesAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"User {userId} not found.");

        return _mapper.Map<List<CatalogueProductDTO>>(user.Favourites);
    }


    public async Task<CatalogueProductDTO> AddFavouriteAsync(Guid userId, AddFavouriteRequest request, CancellationToken cancellationToken = default)
    {
        var catalogueProduct = await _catalogueProductRepository.GetByIdAsync(request.CatalogueProductId, cancellationToken);

        if (catalogueProduct == null)
            throw new NotFoundException($"Catalgue product {request.CatalogueProductId} not found.");

        var user = await _userRepository.GetWithFavouritesAsync(userId, cancellationToken)
           ?? throw new NotFoundException($"User {userId} not found.");

        if (user.Favourites.Any(f => f.Id == catalogueProduct.Id))
            throw new ConflictException($"Product {catalogueProduct.Name} is already in your favourites.");

        user.Favourites.Add(catalogueProduct);

        await _userRepository.UpdateAsync(user, cancellationToken);

       return _mapper.Map<CatalogueProductDTO>(catalogueProduct);
    }

    public async Task RemoveFavouriteAsync(Guid userId, Guid catalogueProductId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetWithFavouritesAsync(userId, cancellationToken)
          ?? throw new NotFoundException($"User {userId} not found.");

         var favourite = user.Favourites.FirstOrDefault(f => f.Id == catalogueProductId) 
            ??  throw new NotFoundException($"Product {catalogueProductId} not found.");

        user.Favourites.Remove(favourite);
        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}
