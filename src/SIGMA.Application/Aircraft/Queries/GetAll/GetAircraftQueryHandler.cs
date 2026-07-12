using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Pagination;

namespace SIGMA.Application.Aircraft.Queries.GetAll;

public class GetAircraftQueryHandler : IRequestHandler<GetAircraftQuery, PaginatedResult<AircraftDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAircraftQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<AircraftDto>> Handle(GetAircraftQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Aircraft
            .Include(a => a.Client)
            .Where(a => !a.IsDeleted)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(a => a.Status == request.Status.Value);

        if (request.Category.HasValue)
            query = query.Where(a => a.Category == request.Category.Value);

        if (request.ClientId.HasValue)
            query = query.Where(a => a.ClientId == request.ClientId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(a =>
                a.Registration.ToLower().Contains(search) ||
                a.Model.ToLower().Contains(search) ||
                a.Manufacturer.ToLower().Contains(search));
        }

        query = query.OrderBy(a => a.Registration);

        var mapped = query.Select(a => new AircraftDto
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
            IsCertificateExpiringSoon = a.CertificateExpiry <= DateTime.UtcNow.AddDays(30),
            ClientId = a.ClientId,
            ClientName = a.Client.Name,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        });

        return await mapped.ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}
