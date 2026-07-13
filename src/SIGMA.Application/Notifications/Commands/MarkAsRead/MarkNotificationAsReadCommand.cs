using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Notifications.Commands.MarkAsRead;

// Comando para marcar una notificacion especifica como leida, validando que pertenezca al usuario
public record MarkNotificationAsReadCommand(Guid NotificationId, Guid UserId) : IRequest<Result>;
