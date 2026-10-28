using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Notifications.Commands.MarkAllAsRead;

// Comando para marcar todas las notificaciones no leidas del usuario actual como leidas
public record MarkAllNotificationsAsReadCommand(Guid UserId) : IRequest<Result>;
