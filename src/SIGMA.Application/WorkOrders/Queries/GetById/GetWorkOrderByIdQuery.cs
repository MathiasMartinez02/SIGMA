using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.WorkOrders.DTOs;

namespace SIGMA.Application.WorkOrders.Queries.GetById;

public record GetWorkOrderByIdQuery(Guid Id) : IRequest<Result<WorkOrderDetailDto>>;
