using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.WorkOrders.DTOs;

namespace SIGMA.Application.WorkOrders.Commands.AddTask;

public record AddWorkOrderTaskCommand(
    Guid WorkOrderId,
    string Title,
    string Description,
    decimal EstimatedHours,
    bool RequiresInspection
) : IRequest<Result<WorkOrderTaskDto>>;
