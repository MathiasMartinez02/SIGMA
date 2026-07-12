using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Clients.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Clients.Queries.GetById;

public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, Result<ClientDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetClientByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ClientDetailDto>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients
            .Include(c => c.Aircraft.Where(a => !a.IsDeleted).OrderBy(a => a.Registration).Take(5))
            .Include(c => c.WorkOrders.Where(w => !w.IsDeleted).OrderByDescending(w => w.CreatedAt).Take(5))
            .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

        if (client is null)
            return Result<ClientDetailDto>.Failure("El cliente no fue encontrado.");

        var aircraftCount = await _context.Aircraft.CountAsync(a => a.ClientId == client.Id && !a.IsDeleted, cancellationToken);
        var workOrderCount = await _context.WorkOrders.CountAsync(w => w.ClientId == client.Id && !w.IsDeleted, cancellationToken);

        var dto = new ClientDetailDto
        {
            Id = client.Id,
            Name = client.Name,
            BusinessName = client.BusinessName,
            TaxId = client.TaxId,
            Email = client.Email,
            Phone = client.Phone,
            Address = client.Address,
            City = client.City,
            Province = client.Province,
            ContactPerson = client.ContactPerson,
            ContactPhone = client.ContactPhone,
            IsActive = client.IsActive,
            AircraftCount = aircraftCount,
            TotalWorkOrders = workOrderCount,
            CreatedAt = client.CreatedAt,
            Aircraft = client.Aircraft.Select(a => new ClientAircraftDto
            {
                Id = a.Id,
                Registration = a.Registration,
                Model = a.Model,
                Status = a.Status.ToString(),
                TotalFlightHours = a.TotalFlightHours
            }).ToList(),
            RecentWorkOrders = client.WorkOrders.Select(w => new ClientWorkOrderDto
            {
                Id = w.Id,
                Number = w.Number,
                Type = w.Type.ToString(),
                Status = w.Status.ToString(),
                CreatedAt = w.CreatedAt
            }).ToList()
        };

        return Result<ClientDetailDto>.Success(dto);
    }
}
