using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Product.Application.DTOs;
using Product.Application.Interfaces;
using Product.Application.Mapping;
using Product.Application.Services;
using Product.Domain.Entities;
using Product.Domain.Exceptions;
using Xunit;

namespace Product.Application.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly IMapper _mapper;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            var mappingConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = mappingConfig.CreateMapper();

            _productService = new ProductService(_mockUnitOfWork.Object, _mapper);
        }

        [Fact]
        public async Task GetProductByIdAsync_WhenProductExists_ReturnsProductDto()
        {
            var productId = 1;
            var product = new Domain.Entities.Product { Id = productId, ProductName = "Test Product" };

            _mockUnitOfWork.Setup(u => u.Products.GetProductWithItemsAsync(productId))
                .ReturnsAsync(product);

            var result = await _productService.GetProductByIdAsync(productId);

            Assert.NotNull(result);
            Assert.Equal("Test Product", result.ProductName);
            Assert.Equal(productId, result.Id);
        }

        [Fact]
        public async Task GetProductByIdAsync_WhenProductDoesNotExist_ThrowsNotFoundException()
        {
            var productId = 1;
            _mockUnitOfWork.Setup(u => u.Products.GetProductWithItemsAsync(productId))
                .ReturnsAsync((Domain.Entities.Product?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _productService.GetProductByIdAsync(productId));
        }

        [Fact]
        public async Task CreateProductAsync_ValidProduct_SavesAndReturnsProductDto()
        {
            var createDto = new CreateProductDto { ProductName = "New Product" };
            
            _mockUnitOfWork.Setup(u => u.Products.AddAsync(It.IsAny<Domain.Entities.Product>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.CompleteAsync())
                .ReturnsAsync(1);

            var result = await _productService.CreateProductAsync(createDto);

            Assert.NotNull(result);
            Assert.Equal("New Product", result.ProductName);
            _mockUnitOfWork.Verify(u => u.Products.AddAsync(It.IsAny<Domain.Entities.Product>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}
