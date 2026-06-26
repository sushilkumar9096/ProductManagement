using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Product.Domain.Entities;
using Product.Application.Interfaces;

namespace Product.Infrastructure.Data.Repositories
{
    public class ItemRepository : GenericRepository<Item>, IItemRepository
    {
        public ItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Item>> GetItemsByProductIdAsync(int productId)
        {
            return await _context.Items
                .AsNoTracking()
                .Where(i => i.ProductId == productId)
                .ToListAsync();
        }
    }
}
