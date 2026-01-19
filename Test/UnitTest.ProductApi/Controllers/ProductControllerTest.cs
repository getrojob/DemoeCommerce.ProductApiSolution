using ProductApi.Application.Interfaces;
using ProductApi.Presentation.Controllers;
using FakeItEasy;
using ProductApi.Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ProductApi.Application.DTOs;
using eCommerce.SharedLibrary.Responses;

namespace UnitTest.ProductApi.Controllers
{
    public class ProductControllerTest
    {
        private readonly IProduct productInterface;
        private readonly ProductsController productsController;

        public ProductControllerTest()
        {
            // Set up dependencies
            productInterface = A.Fake<IProduct>();

            // Set up System Under Test -  SUT
            productsController = new ProductsController(productInterface);
        }

        // Get All Products
        [Fact]
        public async Task GetProducts_WhenProductExists_ReturnOkResponseWithProducts()
        {
            // Arrange
            var products = new List<Product>()
            {
                new(){Id = 1, Name = "Product 1", Quantity = 10, Price = 100.70m},
                new(){Id = 2, Name = "Product 2", Quantity = 120, Price = 1004.70m}
            };
            // set uo fake response for GetAllAsync method
            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

            // Act
            var result = await productsController.GetProducts();

            // Assets
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedProducts = okResult.Value as IEnumerable<ProductDTO>;
            returnedProducts.Should().NotBeNull();
            returnedProducts.Should().HaveCount(2);
            returnedProducts!.First().Id.Should().Be(1);
            returnedProducts!.Last().Id.Should().Be(2);
        }

        [Fact]
        public async Task GetProducts_WhenNoProductsExist_ReturnNotFoundResponse()
        {
            // Arrange
            var products = new List<Product>();

            // Set up fakse response for GetAllAsync();
            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

            // Act
            var result = await productsController.GetProducts();

            // Assets
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            var message = notFoundResult.Value as string;
            message.Should().Be("No products detected in the database");
        }

        [Fact]
        public async Task GetProduct_WhenProductExists_ReturnOkResponseWithProduct()
        {
            // Arrange
            var products = new Product()
            {
                Id = 1,
                Name = "Product 1",
                Quantity = 10,
                Price = 100.70m
            };

            // Set up fakse response for GetAllAsync();
            A.CallTo(() => productInterface.FindByIdAsync(products.Id)).Returns(products);

            // Act
            var result = await productsController.GetProduct(products.Id);

            // Assets
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedProduct = okResult.Value as ProductDTO;
            returnedProduct.Should().NotBeNull();
            returnedProduct!.Id.Should().Be(products.Id);
            returnedProduct!.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetProduct_WhenNoProductsExist_ReturnNotFoundResponse()
        {
            // Arrange
            var products = new Product()
            {
                Id = 20
            };

            // Set up fakse response for GetAllAsync();
            A.CallTo(() => productInterface.FindByIdAsync(products.Id))
        .Returns(Task.FromResult<Product?>(null));

            // Act
            var result = await productsController.GetProduct(products.Id);

            // Assets
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            var message = notFoundResult.Value as string;
            message.Should().Be("No product found with id: 20");
        }

        //Create Product
        [Fact]
        public async Task CreateProduct_WhenModelStateIsInvalid_ReturnBadResquest()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 34, 67.95m);

            productsController.ModelState.AddModelError("Name", "Required");
            // Act

            var result = await productsController.CreateProduct(productDTO);

            // Asset
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task CreateProduct_WhenCreateIsSucessfull_ReturnOkResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 34, 67.95m);
            var response = new Response(true, "Created");

            // Act
            A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.CreateProduct(productDTO);

            // Asset
            var okResult = result.Result as OkObjectResult;
            okResult!.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;
            responseResult!.Message.Should().Be("Created");
            responseResult!.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task CreateProduct_WhenCreateIsFails_ReturnBadRequestResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 78, 45.36m);
            var response = new Response(false, "Failed");

            // Act
            A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.CreateProduct(productDTO);

            // Asset
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult!.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = badRequestResult.Value as Response;
            responseResult!.Message.Should().Be("Failed");
            responseResult!.Flag.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateProduct_WhenUpdateIsSucessfull_ReturnOkResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 34, 67.95m);
            var response = new Response(true, "Update");

            // Act
            A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.UpdateProduct(productDTO);

            // Asset
            var badRequestResult = result.Result as OkObjectResult;
            badRequestResult!.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = badRequestResult.Value as Response;
            responseResult!.Message.Should().Be("Update");
            responseResult!.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateProduct_WhenUpdateIsFails_ReturnBadRequestResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 78, 45.36m);
            var response = new Response(false, "UpdateFailed");
            // Act
            A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.UpdateProduct(productDTO);
            // Asset
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult!.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var responseResult = badRequestResult.Value as Response;
            responseResult!.Message.Should().Be("UpdateFailed");
            responseResult!.Flag.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteProduct_WhenDeleteIsSucessfull_ReturnOkResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 78, 45.36m);
            var response = new Response(true, "Deleted");

            // Act
            A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.DeleteProduct(productDTO);

            // Asset
            var okResult = result.Result as OkObjectResult;
            okResult!.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;
            responseResult!.Message.Should().Be("Deleted");
            responseResult!.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteProduct_WhenDeleteIsFails_ReturnBadRequestResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 78, 45.36m);
            var response = new Response(false, "DeleteFailed");

            // Act
            A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);
            var result = await productsController.DeleteProduct(productDTO);

            // Asset
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult!.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = badRequestResult.Value as Response;
            responseResult!.Message.Should().Be("DeleteFailed");
            responseResult!.Flag.Should().BeFalse();
        }
    }
}
