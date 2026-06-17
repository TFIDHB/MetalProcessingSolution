using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MetalService, MetalServiceResponseDto>();
            CreateMap<MetalServiceImage, MetalServiceImageResponseDto>();

            CreateMap<UnliquidProduct, UnliquidProductResponseDto>();
            CreateMap<UnliquidProductImage, UnliquidProductImageResponseDto>();

            CreateMap<MetalServiceDto, MetalService>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore());

            CreateMap<UnliquidProductDto, UnliquidProduct>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore());
        }
    }
}
