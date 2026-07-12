using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using DomainEntities = SIGMA.Domain.Entities;

namespace SIGMA.Application.Aircraft.Commands.Create;

public class CreateAircraftCommandHandler : IRequestHandler<CreateAircraftCommand, Result<AircraftDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateAircraftCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<AircraftDto>> Handle(CreateAircraftCommand request, CancellationToken cancellationToken)
    {
        var registrationUpper = request.Registration.ToUpperInvariant();

        var exists = await _context.Aircraft
            .AnyAsync(a => a.Registration.ToUpper() == registrationUpper && !a.IsDeleted, cancellationToken);

        if (exists)
            return Result<AircraftDto>.Failure($"Ya existe una aeronave con la matrícula '{request.Registration}'.");

        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == request.ClientId && !c.IsDeleted, cancellationToken);

        if (client is null)
            return Result<AircraftDto>.Failure("El cliente no fue encontrado.");

        var aircraft = DomainEntities.Aircraft.Create(
            request.Registration, request.Model, request.Manufacturer, request.Year,
            request.Category, request.SerialNumber, request.EngineModel,
            request.EngineSerialNumber, request.TotalFlightHours, request.TotalLandings,
            request.LastInspectionDate, request.NextInspectionDue,
            request.NextInspectionHours, request.CertificateExpiry, request.ClientId);

        _context.Aircraft.Add(aircraft);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<AircraftDto>.Success(new AircraftDto
        {
            Id = aircraft.Id,
            Registration = aircraft.Registration,
            Model = aircraft.Model,
            Manufacturer = aircraft.Manufacturer,
            Year = aircraft.Year,
            Category = aircraft.Category,
            Status = aircraft.Status,
            SerialNumber = aircraft.SerialNumber,
            EngineModel = aircraft.EngineModel,
            EngineSerialNumber = aircraft.EngineSerialNumber,
            TotalFlightHours = aircraft.TotalFlightHours,
            TotalLandings = aircraft.TotalLandings,
            LastInspectionDate = aircraft.LastInspectionDate,
            NextInspectionDue = aircraft.NextInspectionDue,
            NextInspectionHours = aircraft.NextInspectionHours,
            CertificateExpiry = aircraft.CertificateExpiry,
            IsCertificateExpiringSoon = aircraft.IsCertificateExpiringSoon(),
            ClientId = aircraft.ClientId,
            ClientName = client.Name,
            CreatedAt = aircraft.CreatedAt,
            UpdatedAt = aircraft.UpdatedAt
        });
    }
}
