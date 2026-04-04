using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Requests.PreferredStores;
using GroceryPromoApi.Application.Services;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Domain.Exceptions;
using Moq;

namespace GroceryPromoApi.Tests.PreferredStores
{
    public class PreferredStoreServiceTests
    {
        private readonly Mock<IPreferredStoreRepository> _preferredStoreRepository = new();
        private readonly PreferredStoreService _preferredStoreService;

        public PreferredStoreServiceTests()
        {
            _preferredStoreService = new PreferredStoreService(_preferredStoreRepository.Object);
        }

        [Fact]
        public async Task GetPreferredStores_ReturnsListForUser()
        {
            var userId = Guid.NewGuid();
            var supermarketId = Guid.NewGuid();
            var stores = new List<PreferredStore>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    SupermarketId = supermarketId,
                    Supermarket = new Supermarket { Id = supermarketId, Name = "Kaufland", Slug = "kaufland" }
                }
            };

            _preferredStoreRepository.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(stores);

            var result = await _preferredStoreService.GetPreferredStoresAsync(userId);

            Assert.Single(result);
            Assert.Equal("Kaufland", result[0].SupermarketName);
            Assert.Equal("kaufland", result[0].SupermarketSlug);
        }

        [Fact]
        public async Task AddPreferredStore_WhenAlreadyExists_ThrowsConflictException()
        {
            var userId = Guid.NewGuid();
            var supermarketId = Guid.NewGuid();

            _preferredStoreRepository.Setup(r => r.ExistsAsync(userId, supermarketId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ConflictException>(() =>
                _preferredStoreService.AddPreferredStoreAsync(userId, new AddPreferredStoreRequest { SupermarketId = supermarketId }));
        }

        [Fact]
        public async Task AddPreferredStore_NewStore_ReturnsDto()
        {
            var userId = Guid.NewGuid();
            var supermarketId = Guid.NewGuid();
            var request = new AddPreferredStoreRequest { SupermarketId = supermarketId };
            var created = new PreferredStore
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SupermarketId = supermarketId,
                Supermarket = new Supermarket { Id = supermarketId, Name = "Lidl", Slug = "lidl" }
            };

            _preferredStoreRepository.Setup(r => r.ExistsAsync(userId, supermarketId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _preferredStoreRepository.Setup(r => r.AddAsync(It.IsAny<PreferredStore>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _preferredStoreRepository.Setup(r => r.GetByUserAndSupermarketAsync(userId, supermarketId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(created);

            var result = await _preferredStoreService.AddPreferredStoreAsync(userId, request);

            Assert.NotNull(result);
            Assert.Equal("Lidl", result.SupermarketName);
            Assert.Equal(supermarketId, result.SupermarketId);
        }

        [Fact]
        public async Task RemovePreferredStore_WhenNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            var supermarketId = Guid.NewGuid();

            _preferredStoreRepository.Setup(r => r.GetByUserAndSupermarketAsync(userId, supermarketId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PreferredStore?)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _preferredStoreService.RemovePreferredStoreAsync(userId, supermarketId));
        }

        [Fact]
        public async Task RemovePreferredStore_WhenExists_DeletesSuccessfully()
        {
            var userId = Guid.NewGuid();
            var supermarketId = Guid.NewGuid();
            var store = new PreferredStore { Id = Guid.NewGuid(), UserId = userId, SupermarketId = supermarketId };

            _preferredStoreRepository.Setup(r => r.GetByUserAndSupermarketAsync(userId, supermarketId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(store);
            _preferredStoreRepository.Setup(r => r.DeleteAsync(store, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _preferredStoreService.RemovePreferredStoreAsync(userId, supermarketId);

            _preferredStoreRepository.Verify(r => r.DeleteAsync(store, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
