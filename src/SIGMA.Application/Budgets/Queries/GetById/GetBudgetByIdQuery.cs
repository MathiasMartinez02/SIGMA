using MediatR;
using SIGMA.Application.Budgets.DTOs;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Budgets.Queries.GetById;

// Query para obtener el detalle de un presupuesto por id
public record GetBudgetByIdQuery(Guid Id) : IRequest<Result<BudgetDto>>;
