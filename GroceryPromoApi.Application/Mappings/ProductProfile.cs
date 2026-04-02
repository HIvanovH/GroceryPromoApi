using AutoMapper;
using GroceryPromoApi.Application.DTOs.Products;
using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDTO>()
            .ForMember(dest => dest.SupermarketName, opt => opt.MapFrom(src => src.Supermarket.Name));
    }
}
