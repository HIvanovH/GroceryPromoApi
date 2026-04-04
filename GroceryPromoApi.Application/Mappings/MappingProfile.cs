using AutoMapper;
using GroceryPromoApi.Application.DTOs.Favorites;
using GroceryPromoApi.Application.DTOs.Products;
using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDTO>()
            .ForMember(dest => dest.SupermarketName, opt => opt.MapFrom(src => src.Supermarket.Name));

        CreateMap<FavouriteProduct, FavouriteProductDTO>();
    }
}

