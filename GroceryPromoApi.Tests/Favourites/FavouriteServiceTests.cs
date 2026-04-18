using AutoMapper;
using GroceryPromoApi.Application.DTOs.Catalogue;
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
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<ICatalogueProductRepository> _catalogueProductRepository = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly FavouriteService _favouriteService;

        public FavouriteServiceTests()
        {
            _favouriteService = new FavouriteService(
                _userRepository.Object,
                _catalogueProductRepository.Object,
                _mapper.Object);
        }

        // ── GetFavouritesAsync ────────────────────────────────────────────

        [Fact]
        public async Task GetFavourites_UserNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();

            _userRepository.Setup(r => r.GetWithFavouritesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _favouriteService.GetFavouritesAsync(userId));
        }

        [Fact]
        public async Task GetFavourites_ReturnsListOfCatalogueProductDTOs()
        {
            var userId = Guid.NewGuid();
            var catalogueProduct = new CatalogueProduct { Id = Guid.NewGuid(), Name = "Кафе" };
            var user = new User { Id = userId, Favourites = new List<CatalogueProduct> { catalogueProduct } };
            var dtos = new List<CatalogueProductDTO> { new() { Id = catalogueProduct.Id, Name = "Кафе" } };

            _userRepository.Setup(r => r.GetWithFavouritesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mapper.Setup(m => m.Map<List<CatalogueProductDTO>>(user.Favourites))
                .Returns(dtos);

            var result = await _favouriteService.GetFavouritesAsync(userId);

            Assert.Single(result);
            Assert.Equal("Кафе", result[0].Name);
        }

        [Fact]
        public async Task GetFavourites_UserWithNoFavourites_ReturnsEmptyList()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Favourites = new List<CatalogueProduct>() };

            _userRepository.Setup(r => r.GetWithFavouritesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mapper.Setup(m => m.Map<List<CatalogueProductDTO>>(user.Favourites))
                .Returns(new List<CatalogueProductDTO>());

            var result = await _favouriteService.GetFavouritesAsync(userId);

            Assert.Empty(result);
        }

        // ── AddFavouriteAsync ─────────────────────────────────────────────

        [Fact]
        public async Task AddFavourite_CatalogueProductNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            var request = new AddFavouriteRequest { CatalogueProductId = Guid.NewGuid() };

            _catalogueProductRepository.Setup(r => r.GetByIdAsync(request.CatalogueProductId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CatalogueProduct?)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _favouriteService.AddFavouriteAsync(userId, request));
        }

        [Fact]
        public async Task AddFavourite_UserNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            var catalogueProduct = new CatalogueProduct { Id = Guid.NewGuid(), Name = "Кафе" };
            var request = new AddFavouriteRequest { CatalogueProductId = catalogueProduct.Id };

            _catalogueProductRepository.Setup(r => r.GetByIdAsync(request.CatalogueProductId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(catalogueProduct);
            _userRepository.Setup(r => r.GetWithFavouritesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _favouriteService.AddFavouriteAsync(userId, request));
        }

        [Fact]
        public async Task AddFavourite_AlreadyInFavourites_ThrowsConflictException()
        {
            var userId = Guid.NewGuid();
            var catalogueProduct = new CatalogueProduct { Id = Guid.NewGuid(), Name = "Кафе" };
            var user = new User { Id = userId, Favourites = new List<CatalogueProduct> { catalogueProduct } };
            var request = new AddFavouriteRequest { CatalogueProductId = catalogueProduct.Id };

            _catalogueProductRepository.Setup(r => r.GetByIdAsync(request.CatalogueProductId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(catalogueProduct);
            _userRepository.Setup(r => r.GetWithFavouritesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            await Assert.ThrowsAsync<ConflictException>(() =>
                _favouriteService.AddFavouriteAsync(userId, request));
        }

        [Fact]
        public async Task AddFavourite_ValidRequest_AddsThenReturnsDTO()
        {
            var userId = Guid.NewGuid();
            var catalogueProduct = new CatalogueProduct { Id = Guid.NewGuid(), Name = "Кафе" };
            var user = new User { Id = userId, Favourites = new List<CatalogueProduct>() };
            var request = new AddFavouriteRequest { CatalogueProductId = catalogueProduct.Id };
            var dto = new CatalogueProductDTO { Id = catalogueProduct.Id, Name = "Кафе" };

            _catalogueProductRepository.Setup(r => r.GetByIdAsync(request.CatalogueProductId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(catalogueProduct);
            _userRepository.Setup(r => r.GetWithFavouritesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _userRepository.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _mapper.Setup(m => m.Map<CatalogueProductDTO>(catalogueProduct))
                .Returns(dto);

            var result = await _favouriteService.AddFavouriteAsync(userId, request);

            Assert.Equal(catalogueProduct.Id, result.Id);
            Assert.Equal("Кафе", result.Name);
            _userRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }

        // ── RemoveFavouriteAsync ──────────────────────────────────────────

        [Fact]
        public async Task RemoveFavourite_UserNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();

            _userRepository.Setup(r => r.GetWithFavouritesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _favouriteService.RemoveFavouriteAsync(userId, Guid.NewGuid()));
        }

        [Fact]
        public async Task RemoveFavourite_ProductNotInFavourites_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Favourites = new List<CatalogueProduct>() };

            _userRepository.Setup(r => r.GetWithFavouritesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _favouriteService.RemoveFavouriteAsync(userId, Guid.NewGuid()));
        }

        [Fact]
        public async Task RemoveFavourite_ValidRequest_RemovesAndSaves()
        {
            var userId = Guid.NewGuid();
            var catalogueProduct = new CatalogueProduct { Id = Guid.NewGuid(), Name = "Кафе" };
            var user = new User { Id = userId, Favourites = new List<CatalogueProduct> { catalogueProduct } };

            _userRepository.Setup(r => r.GetWithFavouritesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _userRepository.Setup(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _favouriteService.RemoveFavouriteAsync(userId, catalogueProduct.Id);

            Assert.Empty(user.Favourites);
            _userRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
