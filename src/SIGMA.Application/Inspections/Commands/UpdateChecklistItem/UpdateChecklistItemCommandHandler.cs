using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Inspections.Commands.UpdateChecklistItem;

public class UpdateChecklistItemCommandHandler : IRequestHandler<UpdateChecklistItemCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateChecklistItemCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateChecklistItemCommand request, CancellationToken cancellationToken)
    {
        var inspection = await _context.Inspections
            .Include(i => i.ChecklistSections)
                .ThenInclude(s => s.Items)
            .FirstOrDefaultAsync(i => i.Id == request.InspectionId && !i.IsDeleted, cancellationToken);

        if (inspection is null)
            return Result.Failure("La inspección no fue encontrada.");

        var item = inspection.ChecklistSections
            .SelectMany(s => s.Items)
            .FirstOrDefault(it => it.Id == request.ItemId);

        if (item is null)
            return Result.Failure("El ítem del checklist no fue encontrado en esta inspección.");

        var userId = _currentUser.UserId!.Value;

        try
        {
            item.Check(request.Status, request.Observations, userId);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
