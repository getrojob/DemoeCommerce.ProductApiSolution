using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductsController(IProduct productInterface) : ControllerBase
    {

        /// <summary>
        /// Get all products
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDTO>), 200)]
        [ProducesResponseType(typeof(Response), 404)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            // Get all products from repo
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
                return NotFound("No products detected in the database");

            // convert data from entity to DTO and return
            var (_, list) = ProductConversions.FromEntity(null, products);
            return list!.Any() ? Ok(list) : NotFound("No product found");
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductDTO), 200)]
        [ProducesResponseType(typeof(Response), 404)]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            // Get single product from repo
            var product = await productInterface.FindByIdAsync(id);
            if (product is null)
                return NotFound($"No product found with id: {id}");

            // convert data from entity to DTO and return
            var (_product, _) = ProductConversions.FromEntity(product, null);
            return _product is not null ? Ok(_product) : NotFound($"No product found with id: {id}");
        }

        /// <summary>
        /// Create a new product
        /// 
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Response), 201)]
        [ProducesResponseType(typeof(Response), 400)]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product
        )
        {
            // check model state is all data annotation are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // convert data from DTO to entity
            var getEntity = ProductConversions.ToEntity(product);

            // add new product to repo
            var response = await productInterface.CreateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Response), 200)]
        [ProducesResponseType(typeof(Response), 400)]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            // check model state is all data annotation are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // convert data from DTO to entity
            var getEntity = ProductConversions.ToEntity(product);

            // update product in repo
            var response = await productInterface.UpdateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Response), 200)]
        [ProducesResponseType(typeof(Response), 400)]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            // convert data from DTO to entity
            var getEntity = ProductConversions.ToEntity(product);
            // delete product from repo
            var response = await productInterface.DeleteAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
    }
}
