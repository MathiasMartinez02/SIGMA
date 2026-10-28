using MediatR;
using SIGMA.API.Common.Pagination;
using SIGMA.API.Users.DTOs;
using SIGMA.API.Enums;

namespace SIGMA.API.Users.Queries.GetAll;

public record GetUsersQuery(
    int Page = 1,
    int PageSize = 10,
    UserRole? Role = null,
    bool? IsActive = null,
    string? Search = null
) : IRequest<PaginatedResult<UserDto>>;
