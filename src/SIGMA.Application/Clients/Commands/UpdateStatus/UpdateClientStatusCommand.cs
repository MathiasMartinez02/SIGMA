using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Clients.Commands.UpdateStatus;

// Comando que activa o desactiva un cliente segun el valor de IsActive
public record UpdateClientStatusCommand(Guid Id, bool IsActive) : IRequest<Result>;
