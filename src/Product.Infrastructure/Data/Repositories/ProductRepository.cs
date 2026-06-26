using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Product.Domain.Entities;
using Product.Application.DTOs;
using Product.Application.Interfaces;

namespace Product.Infrastructure.Data.Repositories
{
    public class ProductRepository : GenericRepository<Domain.Entities.Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Domain.Entities.Product?> GetProductWithItemsAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PaginatedResult<Domain.Entities.Product>> GetProductsPaginatedAsync(PaginationQuery query)
        {
            var source = _context.Products.AsNoTracking().Include(p => p.Items);
            var count = await source.CountAsync();
            var items = await source
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PaginatedResult<Domain.Entities.Product>(items, count, query.PageNumber, query.PageSize);
        }
    }
}
