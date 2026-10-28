using MediatR;
using SIGMA.API.Clients.DTOs;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Clients.Queries.GetById;

public record GetClientByIdQuery(Guid Id) : IRequest<Result<ClientDetailDto>>;
