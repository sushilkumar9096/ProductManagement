using System.Threading.Tasks;
using Product.Domain.Entities;
using Product.Application.DTOs;

namespace Product.Application.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product.Domain.Entities.Product>
    {
        Task<Product.Domain.Entities.Product?> GetProductWithItemsAsync(int id);
        Task<PaginatedResult<Product.Domain.Entities.Product>> GetProductsPaginatedAsync(PaginationQuery query);
    }
}
