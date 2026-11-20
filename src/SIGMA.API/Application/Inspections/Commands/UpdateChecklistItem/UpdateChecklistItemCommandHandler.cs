using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Exceptions;

namespace SIGMA.API.Inspections.Commands.UpdateChecklistItem;

public class UpdateChecklistItemCommandHandler : IRequestHandler<UpdateChecklistItemCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateChecklistItemCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    // Control de un item del checklist de una inspeccion en proceso
    public async Task<Result> Handle(UpdateChecklistItemCommand request, CancellationToken cancellationToken)
    {
        var inspection = await _context.Inspections
            .Include(i => i.ChecklistSections).ThenInclude(s => s.Items)
            .FirstOrDefaultAsync(i => i.Id == request.InspectionId && !i.IsDeleted, cancellationToken);

        if (inspection is null)
            return Result.Failure("La inspección no fue encontrada.");

        try
        {
            inspection.CheckItem(request.ItemId, request.Status, request.Observations, _currentUser.UserId!.Value);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
