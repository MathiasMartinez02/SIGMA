using AutoMapper;
using SIGMA.API.Inspections.DTOs;
using SIGMA.API.Entities;

namespace SIGMA.API.Common.Mappings;

public class InspectionMappingProfile : Profile
{
    public InspectionMappingProfile()
    {
        CreateMap<Inspection, InspectionDto>()
            .ForMember(d => d.WorkOrderNumber, o => o.MapFrom(s => s.WorkOrder != null ? s.WorkOrder.Number : string.Empty))
            .ForMember(d => d.AircraftRegistration, o => o.MapFrom(s => s.Aircraft != null ? s.Aircraft.Registration : string.Empty))
            .ForMember(d => d.InspectorName, o => o.MapFrom(s => s.Inspector != null ? s.Inspector.FullName : null));

        CreateMap<Inspection, InspectionDetailDto>()
            .IncludeBase<Inspection, InspectionDto>();

        CreateMap<ChecklistSection, ChecklistSectionDto>();
        CreateMap<ChecklistItem, ChecklistItemDto>();
    }
}
