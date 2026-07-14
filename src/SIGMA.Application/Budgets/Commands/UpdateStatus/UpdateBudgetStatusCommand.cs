using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Budgets.Commands.UpdateStatus;

// Comando para aceptar o rechazar un presupuesto (el vencimiento no se setea desde aca, se calcula solo)
public record UpdateBudgetStatusCommand(Guid Id, BudgetStatus NewStatus) : IRequest<Result>;
