using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;
using SIGMA.Domain.Exceptions;

namespace SIGMA.Application.Budgets.Commands.UpdateStatus;

// Handler que aplica Accept() o Reject() sobre el presupuesto segun el nuevo estado solicitado
public class UpdateBudgetStatusCommandHandler : IRequestHandler<UpdateBudgetStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateBudgetStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateBudgetStatusCommand request, CancellationToken cancellationToken)
    {
        if (request.NewStatus != BudgetStatus.Aceptado && request.NewStatus != BudgetStatus.Rechazado)
            return Result.Failure("Desde este endpoint solo se puede aceptar o rechazar el presupuesto.");

        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (budget is null)
            return Result.Failure("Presupuesto no encontrado.");

        try
        {
            if (request.NewStatus == BudgetStatus.Aceptado)
                budget.Accept();
            else
                budget.Reject();

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
