using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Reports.DTOs;

namespace SIGMA.Application.Reports.Queries.AircraftStatus;

public class GetAircraftStatusReportQueryHandler : IRequestHandler<GetAircraftStatusReportQuery, AircraftStatusReportDto>
{
    private readonly IApplicationDbContext _context;

    public GetAircraftStatusReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AircraftStatusReportDto> Handle(GetAircraftStatusReportQuery request, CancellationToken cancellationToken)
    {
        var aircraft = await _context.Aircraft
            .Include(a => a.Client)
            .Where(a => !a.IsDeleted)
            .OrderBy(a => a.Registration)
            .ToListAsync(cancellationToken);

        var dtos = aircraft.Select(a => new AircraftDto
        {
            Id = a.Id,
            Registration = a.Registration,
            Model = a.Model,
            Manufacturer = a.Manufacturer,
            Year = a.Year,
            Category = a.Category,
            Status = a.Status,
            SerialNumber = a.SerialNumber,
            EngineModel = a.EngineModel,
            EngineSerialNumber = a.EngineSerialNumber,
            TotalFlightHours = a.TotalFlightHours,
            TotalLandings = a.TotalLandings,
            LastInspectionDate = a.LastInspectionDate,
            NextInspectionDue = a.NextInspectionDue,
            NextInspectionHours = a.NextInspectionHours,
            CertificateExpiry = a.CertificateExpiry,
            IsCertificateExpiringSoon = a.IsCertificateExpiringSoon(),
            ClientId = a.ClientId,
            ClientName = a.Client.Name,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        }).ToList();

        var byStatus = aircraft
            .GroupBy(a => a.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var byCategory = aircraft
            .GroupBy(a => a.Category.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        return new AircraftStatusReportDto
        {
            Aircraft = dtos,
            ByStatus = byStatus,
            ByCategory = byCategory
        };
    }
}
