using AutoMapper;
using GroceryPromoApi.Application.DTOs.Catalogue;
using GroceryPromoApi.Application.DTOs.Products;
using GroceryPromoApi.Application.DTOs.Supermarkets;
using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDTO>()
            .ForMember(dest => dest.SupermarketName, opt => opt.MapFrom(src => src.Supermarket.Name));

        CreateMap<CatalogueProductOffer, CatalogueProductOfferDTO>()
            .ForMember(dest => dest.SupermarketName, opt => opt.MapFrom(src => src.Supermarket.Name))
            .ForMember(dest => dest.IsOnPromo, opt => opt.MapFrom(src =>
                src.PromoValidUntil.HasValue && src.PromoValidUntil.Value >= DateTime.UtcNow));

        CreateMap<CatalogueProduct, CatalogueProductDTO>()
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.NormalizedQuantity));

        CreateMap<Supermarket, SupermarketDTO>();
    }
}

