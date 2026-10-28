using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.WorkOrders.DTOs;

namespace SIGMA.API.WorkOrders.Queries.GetById;

public record GetWorkOrderByIdQuery(Guid Id) : IRequest<Result<WorkOrderDetailDto>>;
