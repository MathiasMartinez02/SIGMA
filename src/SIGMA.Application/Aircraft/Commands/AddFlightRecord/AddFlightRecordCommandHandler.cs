using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.Aircraft.Commands.AddFlightRecord;

public class AddFlightRecordCommandHandler : IRequestHandler<AddFlightRecordCommand, Result<FlightRecordDto>>
{
    private readonly IApplicationDbContext _context;

    public AddFlightRecordCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<FlightRecordDto>> Handle(AddFlightRecordCommand request, CancellationToken cancellationToken)
    {
        var aircraft = await _context.Aircraft
            .FirstOrDefaultAsync(a => a.Id == request.AircraftId && !a.IsDeleted, cancellationToken);

        if (aircraft is null)
            return Result<FlightRecordDto>.Failure("La aeronave no fue encontrada.");

        var flightRecord = FlightRecord.Create(
            request.AircraftId,
            request.Date,
            request.Duration,
            request.Landings,
            request.Pilot,
            request.Origin,
            request.Destination,
            request.Notes);

        var newTotalHours = aircraft.TotalFlightHours + request.Duration;
        var newTotalLandings = aircraft.TotalLandings + request.Landings;
        aircraft.UpdateFlightHours(newTotalHours, newTotalLandings);

        _context.FlightRecords.Add(flightRecord);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new FlightRecordDto
        {
            Id = flightRecord.Id,
            Date = flightRecord.Date,
            Duration = flightRecord.Duration,
            Landings = flightRecord.Landings,
            Pilot = flightRecord.Pilot,
            Origin = flightRecord.Origin,
            Destination = flightRecord.Destination,
            Notes = flightRecord.Notes
        };

        return Result<FlightRecordDto>.Success(dto);
    }
}
