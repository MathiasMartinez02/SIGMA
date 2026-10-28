using MediatR;
using SIGMA.API.Budgets.DTOs;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Budgets.Queries.GetById;

// Query para obtener el detalle de un presupuesto por id
public record GetBudgetByIdQuery(Guid Id) : IRequest<Result<BudgetDto>>;
