using MediatR;
using SIGMA.API.Clients.DTOs;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Clients.Commands.Create;

public record CreateClientCommand(
    string Name,
    string BusinessName,
    string TaxId,
    string Email,
    string Phone,
    string Address,
    string City,
    string Province,
    string? ContactPerson,
    string? ContactPhone
) : IRequest<Result<ClientDto>>;
