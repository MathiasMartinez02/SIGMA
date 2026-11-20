using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Exceptions;

namespace SIGMA.API.Inspections.Commands.Start;

public class StartInspectionCommandHandler : IRequestHandler<StartInspectionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public StartInspectionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    // Inicio de la inspeccion asignada al usuario que la toma
    public async Task<Result> Handle(StartInspectionCommand request, CancellationToken cancellationToken)
    {
        var inspection = await _context.Inspections
            .Include(i => i.ChecklistSections).ThenInclude(s => s.Items)
            .FirstOrDefaultAsync(i => i.Id == request.InspectionId && !i.IsDeleted, cancellationToken);

        if (inspection is null)
            return Result.Failure("La inspección no fue encontrada.");

        try
        {
            inspection.Start(_currentUser.UserId!.Value);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
