using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Budgets.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Budgets.Queries.GetById;

// Handler que busca un presupuesto por id y resuelve su estado efectivo (Vencido calculado)
public class GetBudgetByIdQueryHandler : IRequestHandler<GetBudgetByIdQuery, Result<BudgetDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBudgetByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<BudgetDto>> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
    {
        var budget = await _context.Budgets
            .Include(b => b.Client)
            .Include(b => b.Aircraft)
            .FirstOrDefaultAsync(b => b.Id == request.Id && !b.IsDeleted, cancellationToken);

        if (budget is null)
            return Result<BudgetDto>.Failure("Presupuesto no encontrado.");

        return Result<BudgetDto>.Success(new BudgetDto
        {
            Id = budget.Id,
            ClientId = budget.ClientId,
            ClientName = budget.Client.Name,
            AircraftId = budget.AircraftId,
            AircraftRegistration = budget.Aircraft?.Registration,
            AppointmentId = budget.AppointmentId,
            Description = budget.Description,
            Amount = budget.Amount,
            ValidUntil = budget.ValidUntil,
            Status = budget.GetEffectiveStatus(),
            Notes = budget.Notes,
            CreatedAt = budget.CreatedAt
        });
    }
}
