using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Notifications.Commands.MarkAllAsRead;

// Comando para marcar todas las notificaciones no leidas del usuario actual como leidas
public record MarkAllNotificationsAsReadCommand(Guid UserId) : IRequest<Result>;
