using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Budgets.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.Budgets.Commands.Create;

// Handler que valida existencia y pertenencia (aeronave y turno deben ser del cliente indicado) y crea el presupuesto en estado Pendiente
public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, Result<BudgetDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateBudgetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<BudgetDto>> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        var clientExists = await _context.Clients
            .AnyAsync(c => c.Id == request.ClientId && !c.IsDeleted, cancellationToken);

        if (!clientExists)
            return Result<BudgetDto>.Failure("El cliente no fue encontrado.");

        if (request.AircraftId.HasValue)
        {
            var aircraft = await _context.Aircraft
                .FirstOrDefaultAsync(a => a.Id == request.AircraftId.Value && !a.IsDeleted, cancellationToken);

            if (aircraft is null)
                return Result<BudgetDto>.Failure("La aeronave no fue encontrada.");

            if (aircraft.ClientId != request.ClientId)
                return Result<BudgetDto>.Failure("La aeronave seleccionada no pertenece al cliente indicado.");
        }

        if (request.AppointmentId.HasValue)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId.Value && !a.IsDeleted, cancellationToken);

            if (appointment is null)
                return Result<BudgetDto>.Failure("El turno indicado no fue encontrado.");

            if (appointment.ClientId != request.ClientId)
                return Result<BudgetDto>.Failure("El turno indicado no pertenece al cliente indicado.");
        }

        var budget = Budget.Create(
            request.ClientId, request.AircraftId, request.AppointmentId,
            request.Description, request.Amount, request.Notes);

        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.Budgets
            .Include(b => b.Client)
            .Include(b => b.Aircraft)
            .FirstAsync(b => b.Id == budget.Id, cancellationToken);

        return Result<BudgetDto>.Success(new BudgetDto
        {
            Id = created.Id,
            ClientId = created.ClientId,
            ClientName = created.Client.Name,
            AircraftId = created.AircraftId,
            AircraftRegistration = created.Aircraft?.Registration,
            AppointmentId = created.AppointmentId,
            Description = created.Description,
            Amount = created.Amount,
            ValidUntil = created.ValidUntil,
            Status = created.GetEffectiveStatus(),
            Notes = created.Notes,
            CreatedAt = created.CreatedAt
        });
    }
}
