using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Application.DTOs;
using Product.Application.Interfaces;

namespace Product.Api.Controllers.v1
{
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

        [HttpGet("api/v{version:apiVersion}/products/{productId}/items")]
        public async Task<IActionResult> GetItemsByProduct(int productId)
        {
            var result = await _itemService.GetItemsByProductIdAsync(productId);
            return Ok(result);
        }

        [HttpGet("api/v{version:apiVersion}/items/{id}", Name = "GetItem")]
        public async Task<IActionResult> GetItem(int id)
        {
            var result = await _itemService.GetItemByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("api/v{version:apiVersion}/items")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemDto createItemDto)
        {
            var result = await _itemService.CreateItemAsync(createItemDto);
            return CreatedAtRoute("GetItem", new { id = result.Id }, result);
        }

        [HttpPut("api/v{version:apiVersion}/items/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateItemDto updateItemDto)
        {
            var result = await _itemService.UpdateItemAsync(id, updateItemDto);
            return Ok(result);
        }

        [HttpDelete("api/v{version:apiVersion}/items/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _itemService.DeleteItemAsync(id);
            return NoContent();
        }
    }
}
