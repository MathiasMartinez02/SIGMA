using AutoMapper;
using SIGMA.API.Clients.DTOs;
using DomainEntities = SIGMA.API.Entities;

namespace SIGMA.API.Common.Mappings;

public class ClientMappingProfile : Profile
{
    public ClientMappingProfile()
    {
        CreateMap<DomainEntities.Client, ClientDto>()
            .ForMember(d => d.AircraftCount, o => o.MapFrom(s => s.Aircraft.Count))
            .ForMember(d => d.TotalWorkOrders, o => o.MapFrom(s => s.WorkOrders.Count));

        CreateMap<DomainEntities.Client, ClientDetailDto>()
            .IncludeBase<DomainEntities.Client, ClientDto>();

        CreateMap<DomainEntities.Aircraft, ClientAircraftDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<DomainEntities.WorkOrder, ClientWorkOrderDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
    }
}
