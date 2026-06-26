using System.Threading.Tasks;
using Product.Application.DTOs;

namespace Product.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<PaginatedResult<ProductDto>> GetProductsPaginatedAsync(PaginationQuery query);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
        Task DeleteProductAsync(int id);
    }
}
