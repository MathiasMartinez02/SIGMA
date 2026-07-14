using MediatR;
using SIGMA.Application.Budgets.DTOs;
using SIGMA.Application.Common.Pagination;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Budgets.Queries.GetAll;

// Query paginada de presupuestos con filtros opcionales por estado (efectivo, incluye Vencido calculado) y cliente
public record GetBudgetsQuery(
    int Page = 1,
    int PageSize = 10,
    BudgetStatus? Status = null,
    Guid? ClientId = null
) : IRequest<PaginatedResult<BudgetDto>>;
