using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.Budgets.Commands.UpdateStatus;

// Comando para aceptar o rechazar un presupuesto (el vencimiento no se setea desde aca, se calcula solo)
public record UpdateBudgetStatusCommand(Guid Id, BudgetStatus NewStatus) : IRequest<Result>;
