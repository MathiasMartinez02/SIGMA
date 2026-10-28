using MediatR;
using SIGMA.API.Budgets.DTOs;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Budgets.Commands.Create;

// Comando para registrar un presupuesto para un cliente, con aeronave y turno de origen opcionales
public record CreateBudgetCommand(
    Guid ClientId,
    Guid? AircraftId,
    Guid? AppointmentId,
    string Description,
    decimal Amount,
    string? Notes
) : IRequest<Result<BudgetDto>>;
