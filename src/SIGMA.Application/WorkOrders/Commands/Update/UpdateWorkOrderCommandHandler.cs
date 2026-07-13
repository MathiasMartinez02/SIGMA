using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.WorkOrders.Commands.Update;

public class UpdateWorkOrderCommandHandler : IRequestHandler<UpdateWorkOrderCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateWorkOrderCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result> Handle(UpdateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _context.WorkOrders
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (workOrder is null)
            return Result.Failure("Orden de trabajo no encontrada.");

        workOrder.Update(request.Description, request.EstimatedHours, request.IntakeDate, request.EstimatedEndDate, request.Priority);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
