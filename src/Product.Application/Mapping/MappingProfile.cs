using AutoMapper;
using Product.Domain.Entities;
using Product.Application.DTOs;

namespace Product.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product.Domain.Entities.Product, ProductDto>().ReverseMap();
            CreateMap<CreateProductDto, Product.Domain.Entities.Product>();
            CreateMap<UpdateProductDto, Product.Domain.Entities.Product>();

            CreateMap<Item, ItemDto>().ReverseMap();
            CreateMap<CreateItemDto, Item>();
            CreateMap<UpdateItemDto, Item>();
        }
    }
}
