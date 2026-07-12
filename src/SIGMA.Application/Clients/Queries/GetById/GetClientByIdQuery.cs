using MediatR;
using SIGMA.Application.Clients.DTOs;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Clients.Queries.GetById;

public record GetClientByIdQuery(Guid Id) : IRequest<Result<ClientDetailDto>>;
