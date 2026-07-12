using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.Aircraft.Commands.AddComponent;

public class AddAircraftComponentCommandHandler : IRequestHandler<AddAircraftComponentCommand, Result<AircraftComponentDto>>
{
    private readonly IApplicationDbContext _context;

    public AddAircraftComponentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AircraftComponentDto>> Handle(AddAircraftComponentCommand request, CancellationToken cancellationToken)
    {
        var aircraftExists = await _context.Aircraft
            .AnyAsync(a => a.Id == request.AircraftId && !a.IsDeleted, cancellationToken);

        if (!aircraftExists)
            return Result<AircraftComponentDto>.Failure("La aeronave no fue encontrada.");

        var component = AircraftComponent.Create(
            request.AircraftId,
            request.Name,
            request.PartNumber,
            request.SerialNumber,
            request.Manufacturer,
            request.InstallDate,
            request.InstallHours,
            request.LifeLimitHours,
            request.OverhaulDueHours);

        _context.AircraftComponents.Add(component);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new AircraftComponentDto
        {
            Id = component.Id,
            Name = component.Name,
            PartNumber = component.PartNumber,
            SerialNumber = component.SerialNumber,
            Manufacturer = component.Manufacturer,
            InstallDate = component.InstallDate,
            InstallHours = component.InstallHours,
            LifeLimitHours = component.LifeLimitHours,
            OverhaulDueHours = component.OverhaulDueHours,
            Status = component.Status
        };

        return Result<AircraftComponentDto>.Success(dto);
    }
}
