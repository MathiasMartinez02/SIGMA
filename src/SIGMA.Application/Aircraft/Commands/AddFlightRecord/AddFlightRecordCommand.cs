using MediatR;
using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Aircraft.Commands.AddFlightRecord;

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
