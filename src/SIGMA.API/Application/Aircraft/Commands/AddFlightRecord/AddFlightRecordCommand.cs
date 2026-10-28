using MediatR;
using SIGMA.API.Aircraft.DTOs;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Aircraft.Commands.AddFlightRecord;

public record AddFlightRecordCommand(
    Guid AircraftId,
    DateTime Date,
    decimal Duration,
    int Landings,
    string Pilot,
    string Origin,
    string Destination,
    string? Notes
) : IRequest<Result<FlightRecordDto>>;
