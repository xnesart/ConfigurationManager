using AutoMapper;
using ConfigurationManager.Core.DTOs;
using ConfigurationManager.Core.Models.Requests;

namespace ConfigurationManager.Core.Mapping;

public class RequestMapperProfile:Profile
{
    public RequestMapperProfile()
    {
        CreateMap<AddConfigurationForServiceRequest, ServiceConfigurationDto>();
    }
}