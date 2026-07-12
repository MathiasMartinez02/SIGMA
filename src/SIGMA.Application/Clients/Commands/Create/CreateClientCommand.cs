using MediatR;
using SIGMA.Application.Clients.DTOs;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Clients.Commands.Create;

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
