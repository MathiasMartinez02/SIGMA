using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.WorkOrders.DTOs;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.WorkOrders.Commands.Create;

public record CreateWorkOrderCommand(
    WorkOrderType Type,
    WorkOrderPriority Priority,
    Guid AircraftId,
    string Description,
    decimal EstimatedHours,
    DateTime EstimatedEndDate
) : IRequest<Result<WorkOrderDto>>;
