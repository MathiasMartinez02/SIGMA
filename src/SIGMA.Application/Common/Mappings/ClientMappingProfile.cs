using AutoMapper;
using SIGMA.Application.Clients.DTOs;
using DomainEntities = SIGMA.Domain.Entities;

namespace SIGMA.Application.Common.Mappings;

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
