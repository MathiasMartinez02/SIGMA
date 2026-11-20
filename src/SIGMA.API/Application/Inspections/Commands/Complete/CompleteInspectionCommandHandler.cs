using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Exceptions;

namespace SIGMA.API.Inspections.Commands.Complete;

public class CompleteInspectionCommandHandler : IRequestHandler<CompleteInspectionCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public CompleteInspectionCommandHandler(IApplicationDbContext context) => _context = context;

    // Cierre del checklist de la inspeccion
    public async Task<Result> Handle(CompleteInspectionCommand request, CancellationToken cancellationToken)
    {
        var inspection = await _context.Inspections
            .Include(i => i.ChecklistSections).ThenInclude(s => s.Items)
            .FirstOrDefaultAsync(i => i.Id == request.InspectionId && !i.IsDeleted, cancellationToken);

        if (inspection is null)
            return Result.Failure("La inspección no fue encontrada.");

        try
        {
            inspection.Complete(request.Observations);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
