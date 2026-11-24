using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.WorkOrders.DTOs;
using SIGMA.API.Enums;

namespace SIGMA.API.WorkOrders.Commands.Create;

public record CreateWorkOrderCommand(
    WorkOrderType Type,
    WorkOrderPriority Priority,
    Guid AircraftId,
    string Description,
    decimal EstimatedHours,
    DateTime IntakeDate,
    DateTime EstimatedEndDate
) : IRequest<Result<WorkOrderDto>>;
