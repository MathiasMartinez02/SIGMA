using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Budgets.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Pagination;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Budgets.Queries.GetAll;

// Handler que filtra y pagina los presupuestos, resolviendo el estado efectivo (Vencido calculado) en la propia proyeccion SQL
public class GetBudgetsQueryHandler : IRequestHandler<GetBudgetsQuery, PaginatedResult<BudgetDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBudgetsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<BudgetDto>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var query = _context.Budgets
            .Include(b => b.Client)
            .Include(b => b.Aircraft)
            .Where(b => !b.IsDeleted)
            .AsQueryable();

        if (request.ClientId.HasValue)
            query = query.Where(b => b.ClientId == request.ClientId.Value);

        // El filtro por estado tiene en cuenta el vencimiento calculado: pedir "Vencido" trae los Pendientes
        // ya vencidos, y pedir "Pendiente" excluye a esos mismos (para no duplicarlos entre ambos filtros)
        if (request.Status.HasValue)
        {
            query = request.Status.Value switch
            {
                BudgetStatus.Vencido => query.Where(b => b.Status == BudgetStatus.Pendiente && b.ValidUntil < now),
                BudgetStatus.Pendiente => query.Where(b => b.Status == BudgetStatus.Pendiente && b.ValidUntil >= now),
                _ => query.Where(b => b.Status == request.Status.Value)
            };
        }

        query = query.OrderByDescending(b => b.CreatedAt);

        var mapped = query.Select(b => new BudgetDto
        {
            Id = b.Id,
            ClientId = b.ClientId,
            ClientName = b.Client.Name,
            AircraftId = b.AircraftId,
            AircraftRegistration = b.Aircraft != null ? b.Aircraft.Registration : null,
            AppointmentId = b.AppointmentId,
            Description = b.Description,
            Amount = b.Amount,
            ValidUntil = b.ValidUntil,
            Status = b.Status == BudgetStatus.Pendiente && b.ValidUntil < now ? BudgetStatus.Vencido : b.Status,
            Notes = b.Notes,
            CreatedAt = b.CreatedAt
        });

        return await mapped.ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}
