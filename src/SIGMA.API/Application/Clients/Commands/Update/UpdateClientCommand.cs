using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Clients.Commands.Update;

public record UpdateClientCommand(
    Guid Id,
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
) : IRequest<Result>;
