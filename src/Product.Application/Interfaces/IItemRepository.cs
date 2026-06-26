using System.Collections.Generic;
using System.Threading.Tasks;
using Product.Domain.Entities;

namespace Product.Application.Interfaces
{
    public interface IItemRepository : IGenericRepository<Item>
    {
        Task<IEnumerable<Item>> GetItemsByProductIdAsync(int productId);
    }
}
