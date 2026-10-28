using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Clients.Commands.UpdateStatus;

// Comando que activa o desactiva un cliente segun el valor de IsActive
public record UpdateClientStatusCommand(Guid Id, bool IsActive) : IRequest<Result>;
