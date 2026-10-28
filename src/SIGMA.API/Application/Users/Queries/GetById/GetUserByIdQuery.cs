using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Users.DTOs;

namespace SIGMA.API.Users.Queries.GetById;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDto>>;
