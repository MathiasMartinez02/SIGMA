using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Aircraft.Commands.Update;

public class UpdateAircraftCommandHandler : IRequestHandler<UpdateAircraftCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateAircraftCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateAircraftCommand request, CancellationToken cancellationToken)
    {
        var aircraft = await _context.Aircraft
            .FirstOrDefaultAsync(a => a.Id == request.Id && !a.IsDeleted, cancellationToken);

        if (aircraft is null)
            return Result.Failure("La aeronave no fue encontrada.");

        aircraft.Update(
            request.Model,
            request.Manufacturer,
            request.Year,
            request.Category,
            request.SerialNumber,
            request.EngineModel,
            request.EngineSerialNumber,
            request.TotalFlightHours,
            request.TotalLandings,
            request.LastInspectionDate,
            request.NextInspectionDue,
            request.NextInspectionHours,
            request.CertificateExpiry);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
