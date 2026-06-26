using System.Collections.Generic;
using System.Threading.Tasks;
using Product.Application.DTOs;

namespace Product.Application.Interfaces
{
    public interface IItemService
    {
        Task<ItemDto> GetItemByIdAsync(int id);
        Task<IEnumerable<ItemDto>> GetItemsByProductIdAsync(int productId);
        Task<ItemDto> CreateItemAsync(CreateItemDto createItemDto);
        Task<ItemDto> UpdateItemAsync(int id, UpdateItemDto updateItemDto);
        Task DeleteItemAsync(int id);
    }
}
