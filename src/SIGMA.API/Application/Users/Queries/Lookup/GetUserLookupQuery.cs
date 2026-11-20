using MediatR;
using SIGMA.API.Enums;

namespace SIGMA.API.Users.Queries.Lookup;

public record GetUserLookupQuery(IReadOnlyList<UserRole> Roles) : IRequest<IReadOnlyList<UserLookupDto>>;

public record UserLookupDto(Guid Id, string FullName, UserRole Role, string? LicenseNumber);
