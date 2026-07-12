using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Clients.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Pagination;

namespace SIGMA.Application.Clients.Queries.GetAll;

public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, PaginatedResult<ClientDto>>
{
    private readonly IApplicationDbContext _context;

    public GetClientsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Clients
            .Where(c => !c.IsDeleted)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(c => c.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(search) ||
                c.BusinessName.ToLower().Contains(search) ||
                c.TaxId.ToLower().Contains(search) ||
                c.Email.ToLower().Contains(search));
        }

        query = query.OrderBy(c => c.Name);

        var mapped = query.Select(c => new ClientDto
        {
            Id = c.Id,
            Name = c.Name,
            BusinessName = c.BusinessName,
            TaxId = c.TaxId,
            Email = c.Email,
            Phone = c.Phone,
            Address = c.Address,
            City = c.City,
            Province = c.Province,
            ContactPerson = c.ContactPerson,
            ContactPhone = c.ContactPhone,
            IsActive = c.IsActive,
            AircraftCount = c.Aircraft.Count(a => !a.IsDeleted),
            TotalWorkOrders = c.WorkOrders.Count(w => !w.IsDeleted),
            CreatedAt = c.CreatedAt
        });

        return await mapped.ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}
