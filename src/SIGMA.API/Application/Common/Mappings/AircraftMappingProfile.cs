using AutoMapper;
using SIGMA.API.Aircraft.DTOs;
using DomainEntities = SIGMA.API.Entities;

namespace SIGMA.API.Common.Mappings;

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
