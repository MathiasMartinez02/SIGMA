using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Pagination;
using SIGMA.Application.Users.DTOs;

namespace SIGMA.Application.Users.Queries.GetAll;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedResult<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users
            .Where(u => !u.IsDeleted)
            .AsQueryable();

        if (request.Role.HasValue)
            query = query.Where(u => u.Role == request.Role.Value);

        if (request.IsActive.HasValue)
            query = query.Where(u => u.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search) ||
                u.Email.ToLower().Contains(search));
        }

        query = query.OrderBy(u => u.LastName).ThenBy(u => u.FirstName);

        var mapped = query.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            FullName = u.FirstName + " " + u.LastName,
            Role = u.Role,
            LicenseNumber = u.LicenseNumber,
            LicenseExpiry = u.LicenseExpiry,
            Phone = u.Phone,
            AvatarUrl = u.AvatarUrl,
            IsActive = u.IsActive,
            LastLoginAt = u.LastLoginAt,
            CreatedAt = u.CreatedAt
        });

        return await mapped.ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}
