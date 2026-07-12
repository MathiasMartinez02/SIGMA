using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Clients.Commands.Update;

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
