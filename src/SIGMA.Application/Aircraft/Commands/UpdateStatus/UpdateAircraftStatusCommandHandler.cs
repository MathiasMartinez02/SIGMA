using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Aircraft.Commands.UpdateStatus;

public class UpdateAircraftStatusCommandHandler : IRequestHandler<UpdateAircraftStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateAircraftStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateAircraftStatusCommand request, CancellationToken cancellationToken)
    {
        var aircraft = await _context.Aircraft
            .FirstOrDefaultAsync(a => a.Id == request.Id && !a.IsDeleted, cancellationToken);

        if (aircraft is null)
            return Result.Failure("La aeronave no fue encontrada.");

        aircraft.UpdateStatus(request.Status);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
