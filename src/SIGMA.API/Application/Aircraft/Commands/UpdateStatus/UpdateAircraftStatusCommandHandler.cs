using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.WorkOrders.Services;
using SIGMA.API.Enums;

namespace SIGMA.API.Aircraft.Commands.UpdateStatus;

public class UpdateAircraftStatusCommandHandler : IRequestHandler<UpdateAircraftStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateAircraftStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    // Fuera de servicio o vuelta a servicio; los estados de mantenimiento salen de las OT activas
    public async Task<Result> Handle(UpdateAircraftStatusCommand request, CancellationToken cancellationToken)
    {
        var aircraft = await _context.Aircraft
            .FirstOrDefaultAsync(a => a.Id == request.Id && !a.IsDeleted, cancellationToken);

        if (aircraft is null)
            return Result.Failure("La aeronave no fue encontrada.");

        switch (request.Status)
        {
            case AircraftStatus.FueraDeServicio:
                aircraft.UpdateStatus(AircraftStatus.FueraDeServicio);
                break;
            case AircraftStatus.Operativa:
                aircraft.UpdateStatus(AircraftStatus.Operativa);
                await AircraftStatusSync.ApplyAsync(_context, aircraft, cancellationToken);
                break;
            default:
                return Result.Failure("En mantenimiento y En inspección se asignan solos según las OT activas de la aeronave.");
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
