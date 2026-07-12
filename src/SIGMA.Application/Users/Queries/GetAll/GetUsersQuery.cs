using MediatR;
using SIGMA.Application.Common.Pagination;
using SIGMA.Application.Users.DTOs;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Users.Queries.GetAll;

public record GetUsersQuery(
    int Page = 1,
    int PageSize = 10,
    UserRole? Role = null,
    bool? IsActive = null,
    string? Search = null
) : IRequest<PaginatedResult<UserDto>>;
