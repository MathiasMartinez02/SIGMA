using System.Linq.Expressions;
using SIGMA.API.Entities;
using SIGMA.API.Enums;

namespace SIGMA.API.WorkOrders.DTOs;

public static class WorkOrderProjections
{
    public static readonly Expression<Func<WorkOrder, WorkOrderDto>> ToDto = w => new WorkOrderDto
    {
        Id = w.Id,
        Number = w.Number,
        Type = w.Type,
        Status = w.Status,
        Priority = w.Priority,
        AircraftId = w.AircraftId,
        AircraftRegistration = w.Aircraft.Registration,
        AircraftModel = w.Aircraft.Model,
        ClientId = w.ClientId,
        ClientName = w.Client.Name,
        Description = w.Description,
        EstimatedHours = w.EstimatedHours,
        ActualHours = w.ActualHours,
        StartDate = w.StartDate,
        IntakeDate = w.IntakeDate,
        EstimatedEndDate = w.EstimatedEndDate,
        CompletedDate = w.CompletedDate,
        AircraftHoursAtStart = w.AircraftHoursAtStart,
        CreatedAt = w.CreatedAt,
        UpdatedAt = w.UpdatedAt,
        TaskCount = w.Tasks.Count,
        CompletedTaskCount = w.Tasks.Count(t => t.Status == WorkOrderTaskStatus.Completada)
    };
}
