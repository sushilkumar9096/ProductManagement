using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Product.Application.DTOs;
using Product.Application.Interfaces;

namespace Product.Api.Controllers.v1
{
    /// <summary>
    /// Handles CRUD operations for Products.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Retrieves a paginated list of products.
        /// </summary>
        /// <param name="query">Pagination parameters (page number and page size).</param>
        /// <returns>A paginated list of products.</returns>
        /// <response code="200">Returns the paginated list of products.</response>
        /// <response code="401">If the request is unauthorized.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProducts([FromQuery] PaginationQuery query)
        {
            var result = await _productService.GetProductsPaginatedAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific product by its unique identifier.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The product details if found.</returns>
        /// <response code="200">Returns the requested product.</response>
        /// <response code="401">If the request is unauthorized.</response>
        /// <response code="404">If the product is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new product. Only accessible by users with the Administrator role.
        /// </summary>
        /// <param name="createProductDto">The product creation details.</param>
        /// <returns>The created product details.</returns>
        /// <response code="201">Returns the created product details.</response>
        /// <response code="400">If the input details are invalid.</response>
        /// <response code="401">If the request is unauthorized.</response>
        /// <response code="403">If the user is not in the Administrator role.</response>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            var result = await _productService.CreateProductAsync(createProductDto);
            return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing product details. Only accessible by users with the Administrator role.
        /// </summary>
        /// <param name="id">The product ID to update.</param>
        /// <param name="updateProductDto">The updated details of the product.</param>
        /// <returns>The updated product details.</returns>
        /// <response code="200">Returns the updated product details.</response>
        /// <response code="400">If the input details are invalid.</response>
        /// <response code="401">If the request is unauthorized.</response>
        /// <response code="403">If the user is not in the Administrator role.</response>
        /// <response code="404">If the product is not found.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            var result = await _productService.UpdateProductAsync(id, updateProductDto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a specific product by its ID. Only accessible by users with the Administrator role.
        /// </summary>
        /// <param name="id">The product ID to delete.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">If the product was successfully deleted.</response>
        /// <response code="401">If the request is unauthorized.</response>
        /// <response code="403">If the user is not in the Administrator role.</response>
        /// <response code="404">If the product is not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
