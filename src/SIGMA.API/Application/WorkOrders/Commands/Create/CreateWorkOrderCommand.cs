using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.WorkOrders.DTOs;
using SIGMA.API.Enums;

namespace SIGMA.API.WorkOrders.Commands.Create;

// MODIFICADO: agrega IntakeDate (fecha de ingreso de la aeronave al taller) requerida al crear la OT
// ANTERIOR: CreateWorkOrderCommand(WorkOrderType Type, WorkOrderPriority Priority, Guid AircraftId, string Description, decimal EstimatedHours, DateTime EstimatedEndDate) sin fecha de ingreso
public record CreateWorkOrderCommand(
    WorkOrderType Type,
    WorkOrderPriority Priority,
    Guid AircraftId,
    string Description,
    decimal EstimatedHours,
    DateTime IntakeDate,
    DateTime EstimatedEndDate
) : IRequest<Result<WorkOrderDto>>;
