using AutoMapper;
using Messaging.Shared;
using ServiceConfigManager.Core.DTOs;

namespace ServiceConfigManager.Core.Mapping;

public class RabbitMappingProfile : Profile
{
    public RabbitMappingProfile()
    {
        CreateMap<ServiceConfigurationDto, SettingsModel>()
            .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
            .ForMember(dest => dest.Settings,
                opt => opt.MapFrom(src => new Dictionary<string, string> { { src.Key, src.Value } }));
    }
}