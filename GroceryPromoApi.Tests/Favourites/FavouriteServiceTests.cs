using AutoMapper;
using GroceryPromoApi.Application.DTOs.Favorites;
using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Requests.Favourites;
using GroceryPromoApi.Application.Services;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Domain.Exceptions;
using Moq;

namespace GroceryPromoApi.Tests.Favourites
{
    public class FavouriteServiceTests
    {
        private readonly Mock<IFavouriteRepository> _favouriteRepository = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly FavouriteService _favouriteService;

        public FavouriteServiceTests()
        {
            _favouriteService = new FavouriteService(_favouriteRepository.Object, _mapper.Object);
        }

        [Fact]
        public async Task GetFavourites_ReturnsListForUser()
        {
            var userId = Guid.NewGuid();
            var favourites = new List<FavouriteProduct>
            {
                new() { Id = Guid.NewGuid(), UserId = userId, NormalizedName = "кафе" }
            };
            var dtos = new List<FavouriteProductDTO>
            {
                new() { Id = favourites[0].Id, NormalizedName = "кафе" }
            };

            _favouriteRepository.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(favourites);
            _mapper.Setup(m => m.Map<List<FavouriteProductDTO>>(favourites))
                .Returns(dtos);

            var result = await _favouriteService.GetFavouritesAsync(userId);

            Assert.Single(result);
            Assert.Equal("кафе", result[0].NormalizedName);
        }

        [Fact]
        public async Task AddFavourite_WhenAlreadyExists_ThrowsConflictException()
        {
            var userId = Guid.NewGuid();
            var request = new AddFavouriteRequest
            {
                NormalizedName = "кафе",
                NormalizedQuantity = "500г"
            };

            _favouriteRepository.Setup(r => r.ExistsAsync(userId, request.NormalizedName, request.NormalizedQuantity, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ConflictException>(() =>
                _favouriteService.AddFavouriteAsync(userId, request));
        }

        [Fact]
        public async Task AddFavourite_NewProduct_ReturnsDto()
        {
            var userId = Guid.NewGuid();
            var request = new AddFavouriteRequest
            {
                NormalizedName = "кафе",
                NormalizedQuantity = "500г",
                Category = "Напитки"
            };
            var dto = new FavouriteProductDTO { NormalizedName = "кафе" };

            _favouriteRepository.Setup(r => r.ExistsAsync(userId, request.NormalizedName, request.NormalizedQuantity, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _favouriteRepository.Setup(r => r.AddAsync(It.IsAny<FavouriteProduct>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _mapper.Setup(m => m.Map<FavouriteProductDTO>(It.IsAny<FavouriteProduct>()))
                .Returns(dto);

            var result = await _favouriteService.AddFavouriteAsync(userId, request);

            Assert.NotNull(result);
            Assert.Equal("кафе", result.NormalizedName);
        }

        [Fact]
        public async Task RemoveFavourite_WhenNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            var favouriteId = Guid.NewGuid();

            _favouriteRepository.Setup(r => r.GetByIdAsync(favouriteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((FavouriteProduct?)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _favouriteService.RemoveFavouriteAsync(userId, favouriteId));
        }

        [Fact]
        public async Task RemoveFavourite_WhenNotOwner_ThrowsForbiddenException()
        {
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var favouriteId = Guid.NewGuid();

            _favouriteRepository.Setup(r => r.GetByIdAsync(favouriteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FavouriteProduct { Id = favouriteId, UserId = otherUserId });

            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _favouriteService.RemoveFavouriteAsync(userId, favouriteId));
        }

        [Fact]
        public async Task RemoveFavourite_WhenOwner_DeletesSuccessfully()
        {
            var userId = Guid.NewGuid();
            var favouriteId = Guid.NewGuid();
            var favourite = new FavouriteProduct { Id = favouriteId, UserId = userId };

            _favouriteRepository.Setup(r => r.GetByIdAsync(favouriteId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(favourite);
            _favouriteRepository.Setup(r => r.DeleteAsync(favourite, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _favouriteService.RemoveFavouriteAsync(userId, favouriteId);

            _favouriteRepository.Verify(r => r.DeleteAsync(favourite, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
