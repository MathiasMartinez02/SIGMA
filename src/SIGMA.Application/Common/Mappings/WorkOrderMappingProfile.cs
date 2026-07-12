using AutoMapper;
using SIGMA.Application.WorkOrders.DTOs;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.Common.Mappings;

public class WorkOrderMappingProfile : Profile
{
    public WorkOrderMappingProfile()
    {
        CreateMap<WorkOrder, WorkOrderDto>()
            .ForMember(d => d.AircraftRegistration, o => o.MapFrom(s => s.Aircraft.Registration))
            .ForMember(d => d.AircraftModel, o => o.MapFrom(s => s.Aircraft.Model))
            .ForMember(d => d.ClientName, o => o.MapFrom(s => s.Client.Name))
            .ForMember(d => d.TaskCount, o => o.MapFrom(s => s.Tasks.Count))
            .ForMember(d => d.CompletedTaskCount, o => o.MapFrom(s =>
                s.Tasks.Count(t => t.Status == Domain.Enums.WorkOrderTaskStatus.Completada)));

        CreateMap<WorkOrder, WorkOrderDetailDto>()
            .IncludeBase<WorkOrder, WorkOrderDto>();

        CreateMap<WorkOrderTask, WorkOrderTaskDto>()
            .ForMember(d => d.AssignedToName, o => o.MapFrom(s =>
                s.AssignedTo != null ? s.AssignedTo.FullName : null))
            .ForMember(d => d.InspectedByName, o => o.MapFrom(s =>
                s.InspectedBy != null ? s.InspectedBy.FullName : null));

        CreateMap<WorkOrderTimeline, WorkOrderTimelineDto>();
        CreateMap<WorkOrderMaterial, WorkOrderMaterialDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
        CreateMap<WorkOrderDocument, WorkOrderDocumentDto>();
        CreateMap<AssignedMechanic, AssignedMechanicDto>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.FullName));
    }
}
