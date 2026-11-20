using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;

namespace SIGMA.API.Users.Queries.Lookup;

public class GetUserLookupQueryHandler : IRequestHandler<GetUserLookupQuery, IReadOnlyList<UserLookupDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUserLookupQueryHandler(IApplicationDbContext context) => _context = context;

    // Usuarios activos (opcionalmente filtrados por rol) para combos de asignacion
    public async Task<IReadOnlyList<UserLookupDto>> Handle(GetUserLookupQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users.Where(u => u.IsActive && !u.IsDeleted);
        if (request.Roles.Count != 0)
            query = query.Where(u => request.Roles.Contains(u.Role));

        return await query
            .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
            .Select(u => new UserLookupDto(u.Id, u.FirstName + " " + u.LastName, u.Role, u.LicenseNumber))
            .ToListAsync(cancellationToken);
    }
}
