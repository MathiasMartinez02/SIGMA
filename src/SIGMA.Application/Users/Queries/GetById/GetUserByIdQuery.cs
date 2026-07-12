using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Users.DTOs;

namespace SIGMA.Application.Users.Queries.GetById;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDto>>;
