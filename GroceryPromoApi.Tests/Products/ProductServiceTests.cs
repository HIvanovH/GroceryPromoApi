using AutoMapper;
using GroceryPromoApi.Application.DTOs.Products;
using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Requests.Products;
using GroceryPromoApi.Application.Services;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Domain.Exceptions;
using Moq;

namespace GroceryPromoApi.Tests.Products
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepository = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _productService = new ProductService(_productRepository.Object, _mapper.Object);
        }

        [Fact]
        public async Task Search_ReturnsMatchingProducts()
        {
            var supermarketId = Guid.NewGuid();
            var products = new List<Product>
            {
                new() { Id = Guid.NewGuid(), Name = "Кафе Jacobs", SupermarketId = supermarketId }
            };
            var dtos = new List<ProductDTO>
            {
                new() { Id = products[0].Id, Name = "Кафе Jacobs" }
            };

            _productRepository.Setup(r => r.SearchAsync("кафе", null, null, 1, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);
            _mapper.Setup(m => m.Map<List<ProductDTO>>(products))
                .Returns(dtos);

            var result = await _productService.SearchAsync(new ProductSearchRequest { Name = "кафе" });

            Assert.Single(result);
            Assert.Equal("Кафе Jacobs", result[0].Name);
        }

        [Fact]
        public async Task Search_NoResults_ReturnsEmptyList()
        {
            _productRepository.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());
            _mapper.Setup(m => m.Map<List<ProductDTO>>(It.IsAny<List<Product>>()))
                .Returns(new List<ProductDTO>());

            var result = await _productService.SearchAsync(new ProductSearchRequest { Name = "нещо несъществуващо" });

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetById_ExistingProduct_ReturnsDto()
        {
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Мляко" };
            var dto = new ProductDTO { Id = productId, Name = "Мляко" };

            _productRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _mapper.Setup(m => m.Map<ProductDTO>(product))
                .Returns(dto);

            var result = await _productService.GetByIdAsync(productId);

            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("Мляко", result.Name);
        }

        [Fact]
        public async Task GetById_NotFound_ThrowsNotFoundException()
        {
            var productId = Guid.NewGuid();

            _productRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _productService.GetByIdAsync(productId));
        }

        [Fact]
        public async Task Search_PassesAllFiltersToRepository()
        {
            var supermarketId = Guid.NewGuid();
            var request = new ProductSearchRequest
            {
                Name = "кафе",
                SupermarketId = supermarketId,
                Category = "Напитки",
                Page = 2,
                PageSize = 10
            };

            _productRepository.Setup(r => r.SearchAsync("кафе", supermarketId, "Напитки", 2, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());
            _mapper.Setup(m => m.Map<List<ProductDTO>>(It.IsAny<List<Product>>()))
                .Returns(new List<ProductDTO>());

            await _productService.SearchAsync(request);

            _productRepository.Verify(r => r.SearchAsync("кафе", supermarketId, "Напитки", 2, 10, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
