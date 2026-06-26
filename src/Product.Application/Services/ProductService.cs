using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Product.Domain.Entities;
using Product.Domain.Exceptions;
using Product.Application.DTOs;
using Product.Application.Interfaces;

namespace Product.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetProductWithItemsAsync(id);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {id} was not found.");
            }
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<PaginatedResult<ProductDto>> GetProductsPaginatedAsync(PaginationQuery query)
        {
            var paginatedProducts = await _unitOfWork.Products.GetProductsPaginatedAsync(query);
            var dtos = _mapper.Map<List<ProductDto>>(paginatedProducts.Items);
            return new PaginatedResult<ProductDto>(dtos, paginatedProducts.TotalCount, paginatedProducts.PageNumber, paginatedProducts.PageSize);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Domain.Entities.Product>(createProductDto);
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {id} was not found.");
            }

            _mapper.Map(updateProductDto, product);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {id} was not found.");
            }

            _unitOfWork.Products.Delete(product);
            await _unitOfWork.CompleteAsync();
        }
    }
}
