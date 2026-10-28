using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.WorkOrders.DTOs;

namespace SIGMA.API.WorkOrders.Commands.AddTask;

public record AddWorkOrderTaskCommand(
    Guid WorkOrderId,
    string Title,
    string Description,
    decimal EstimatedHours,
    bool RequiresInspection
) : IRequest<Result<WorkOrderTaskDto>>;
