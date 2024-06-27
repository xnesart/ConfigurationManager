using AutoMapper;
using ServiceConfigManager.Core.DTOs;
using ServiceConfigManager.Core.Models.Requests;

namespace ServiceConfigManager.Core.Mapping;

public class RequestMapperProfile:Profile
{
    public RequestMapperProfile()
    {
        CreateMap<AddConfigurationForServiceRequest, ServiceConfigurationDto>();
    }
}