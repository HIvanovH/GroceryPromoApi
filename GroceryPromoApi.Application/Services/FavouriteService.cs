using AutoMapper;
using GroceryPromoApi.Application.DTOs.Favorites;
using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Interfaces.Services;
using GroceryPromoApi.Application.Requests.Favourites;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Domain.Exceptions;
using System.Collections.Generic;

namespace GroceryPromoApi.Application.Services;

public class FavouriteService : IFavouriteService
{
    private readonly IFavouriteRepository _favouriteRepository;
    private readonly IMapper _mapper;

    public FavouriteService(IFavouriteRepository favouriteRepository, IMapper mapper)
    {
        _favouriteRepository = favouriteRepository;
        _mapper = mapper;
    }

    public async Task<List<FavouriteProductDTO>> GetFavouritesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        List<FavouriteProduct> result =  await _favouriteRepository.GetByUserIdAsync(userId, cancellationToken);
        return _mapper.Map<List<FavouriteProductDTO>>(result);
    }

    public async Task<FavouriteProductDTO> AddFavouriteAsync(Guid userId, AddFavouriteRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await _favouriteRepository.ExistsAsync(
            userId, request.NormalizedName, request.NormalizedQuantity, cancellationToken);

        if (exists)

            throw new ConflictException("This product is already in your favourites.");
        var favourite = new FavouriteProduct
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            NormalizedName = request.NormalizedName,
            NormalizedQuantity = request.NormalizedQuantity,
            Category = request.Category,
            CreatedAt = DateTime.UtcNow
        };

        await _favouriteRepository.AddAsync(favourite, cancellationToken);
        return _mapper.Map<FavouriteProductDTO>(favourite);
    }

    public async Task RemoveFavouriteAsync(Guid userId, Guid favouriteId, CancellationToken cancellationToken = default)
    {
        var favourite = await _favouriteRepository.GetByIdAsync(favouriteId, cancellationToken)
            ?? throw new NotFoundException($"Favourite {favouriteId} not found.");

        if (favourite.UserId != userId)
            throw new ForbiddenException("You do not have permission to delete this favourite.");

        await _favouriteRepository.DeleteAsync(favourite, cancellationToken);
    }
 
}
