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
    /// Handles CRUD operations for Items (which are child entities of Products).
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        /// <summary>
        /// Retrieves all items belonging to a specific product.
        /// </summary>
        /// <param name="productId">The ID of the parent product.</param>
        /// <returns>A list of items associated with the product.</returns>
        /// <response code="200">Returns the list of items.</response>
        /// <response code="401">If the request is unauthorized.</response>
        /// <response code="404">If the product does not exist.</response>
        [HttpGet("api/v{version:apiVersion}/products/{productId}/items")]
        [ProducesResponseType(typeof(System.Collections.Generic.IEnumerable<ItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItemsByProduct(int productId)
        {
            var result = await _itemService.GetItemsByProductIdAsync(productId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific item by its unique identifier.
        /// </summary>
        /// <param name="id">The item ID.</param>
        /// <returns>The item details.</returns>
        /// <response code="200">Returns the requested item details.</response>
        /// <response code="401">If the request is unauthorized.</response>
        /// <response code="404">If the item is not found.</response>
        [HttpGet("api/v{version:apiVersion}/items/{id}", Name = "GetItem")]
        [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItem(int id)
        {
            var result = await _itemService.GetItemByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new item. Only accessible by users with the Administrator role.
        /// </summary>
        /// <param name="createItemDto">The item creation details.</param>
        /// <returns>The created item details.</returns>
        /// <response code="201">Returns the created item details.</response>
        /// <response code="400">If the input details are invalid or the parent product is not found.</response>
        /// <response code="401">If the request is unauthorized.</response>
        /// <response code="403">If the user is not in the Administrator role.</response>
        [HttpPost("api/v{version:apiVersion}/items")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemDto createItemDto)
        {
            var result = await _itemService.CreateItemAsync(createItemDto);
            return CreatedAtRoute("GetItem", new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing item. Only accessible by users with the Administrator role.
        /// </summary>
        /// <param name="id">The item ID to update.</param>
        /// <param name="updateItemDto">The updated details of the item.</param>
        /// <returns>The updated item details.</returns>
        /// <response code="200">Returns the updated item details.</response>
        /// <response code="400">If the input details are invalid.</response>
        /// <response code="401">If the request is unauthorized.</response>
        /// <response code="403">If the user is not in the Administrator role.</response>
        /// <response code="404">If the item is not found.</response>
        [HttpPut("api/v{version:apiVersion}/items/{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateItemDto updateItemDto)
        {
            var result = await _itemService.UpdateItemAsync(id, updateItemDto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a specific item. Only accessible by users with the Administrator role.
        /// </summary>
        /// <param name="id">The item ID to delete.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">If the item was successfully deleted.</response>
        /// <response code="401">If the request is unauthorized.</response>
        /// <response code="403">If the user is not in the Administrator role.</response>
        /// <response code="404">If the item is not found.</response>
        [HttpDelete("api/v{version:apiVersion}/items/{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _itemService.DeleteItemAsync(id);
            return NoContent();
        }
    }
}
