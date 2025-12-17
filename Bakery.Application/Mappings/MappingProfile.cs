using AutoMapper;
using Bakery.Application.DTOs;
using Bakery.Core.Entities.People;
using Bakery.Core.Entities.Products;

namespace Bakery.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping between entities and DTOs
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.ProductType.ToString()))
            .ForMember(dest => dest.MarketId, opt => opt.MapFrom(src => src.MarketId))
            .ForMember(dest => dest.MarketName, opt => opt.Ignore());

        CreateMap<Product, ProductSummaryDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.ProductType.ToString()));

        // Market mappings
        CreateMap<Market, MarketDto>();

        // Customer mappings
        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
    }
}
