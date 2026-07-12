using AutoMapper;
using SIGMA.Application.Aircraft.DTOs;
using DomainEntities = SIGMA.Domain.Entities;

namespace SIGMA.Application.Common.Mappings;

public class AircraftMappingProfile : Profile
{
    public AircraftMappingProfile()
    {
        CreateMap<DomainEntities.Aircraft, AircraftDto>()
            .ForMember(d => d.ClientName, o => o.MapFrom(s => s.Client != null ? s.Client.Name : string.Empty))
            .ForMember(d => d.IsCertificateExpiringSoon, o => o.MapFrom(s => s.CertificateExpiry <= DateTime.UtcNow.AddDays(30)));

        CreateMap<DomainEntities.Aircraft, AircraftDetailDto>()
            .IncludeBase<DomainEntities.Aircraft, AircraftDto>();

        CreateMap<DomainEntities.AircraftDocument, AircraftDocumentDto>();
        CreateMap<DomainEntities.FlightRecord, FlightRecordDto>();
        CreateMap<DomainEntities.AircraftComponent, AircraftComponentDto>();
    }
}
