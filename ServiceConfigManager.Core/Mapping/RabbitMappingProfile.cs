using AutoMapper;
using ServiceConfigManager.Core.DTOs;
using ServiceConfigManager.Core.Models.Rabbit;

namespace ServiceConfigManager.Core.Mapping;

public class RabbitMappingProfile : Profile
{
    public RabbitMappingProfile()
    {
        CreateMap<ServiceConfigurationDto, SettingsModel>()
            .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
            .ForMember(dest => dest.Settigns,
                opt => opt.MapFrom(src => new Dictionary<string, string> { { src.Key, src.Value } }));
    }
}