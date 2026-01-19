using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entity;
using ProductApi.Infraestructure.Data;
using ProductApi.Infraestructure.Repositories;

namespace UnitTest.ProductApi.Repositories
{
    public class ProductRepositoryTest
    {
        private readonly ProductDbContext productDbContext;
        private readonly ProductRepository productRepository;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: "ProductDbTest")
                .Options;
            productDbContext = new ProductDbContext(options);
            productRepository = new ProductRepository(productDbContext);
        }

        [Fact]
        public async Task CreateAsync_WhenProductAlreadyExists_ReturnErrorResponse()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Quantity = 10, Price = 99.99m };

            // Act
            await productRepository.CreateAsync(product);

            // Assert
            var addedProduct = await productDbContext.Products.FirstOrDefaultAsync(p => p.Name == "Test Product");
            Assert.NotNull(addedProduct);
            Assert.Equal(10, addedProduct.Quantity);
            Assert.Equal(99.99m, addedProduct.Price);
        }
    }
}
