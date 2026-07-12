using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Inspections.Commands.Approve;

public class ApproveInspectionCommandHandler : IRequestHandler<ApproveInspectionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ApproveInspectionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ApproveInspectionCommand request, CancellationToken cancellationToken)
    {
        var inspection = await _context.Inspections
            .Include(i => i.WorkOrder)
                .ThenInclude(w => w.Tasks)
            .FirstOrDefaultAsync(i => i.Id == request.InspectionId && !i.IsDeleted, cancellationToken);

        if (inspection is null)
            return Result.Failure("La inspección no fue encontrada.");

        var userId = _currentUser.UserId!.Value;

        try
        {
            inspection.Approve(userId, request.Observations);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
