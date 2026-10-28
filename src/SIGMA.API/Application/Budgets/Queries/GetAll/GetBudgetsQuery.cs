using MediatR;
using SIGMA.API.Budgets.DTOs;
using SIGMA.API.Common.Pagination;
using SIGMA.API.Enums;

namespace SIGMA.API.Budgets.Queries.GetAll;

// Query paginada de presupuestos con filtros opcionales por estado (efectivo, incluye Vencido calculado) y cliente
public record GetBudgetsQuery(
    int Page = 1,
    int PageSize = 10,
    BudgetStatus? Status = null,
    Guid? ClientId = null
) : IRequest<PaginatedResult<BudgetDto>>;
