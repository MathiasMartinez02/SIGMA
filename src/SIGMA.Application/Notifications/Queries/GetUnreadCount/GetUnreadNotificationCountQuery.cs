using MediatR;

namespace SIGMA.Application.Notifications.Queries.GetUnreadCount;

// Query que devuelve solo la cantidad de notificaciones no leidas del usuario, para el badge de la campana
public record GetUnreadNotificationCountQuery(Guid UserId) : IRequest<int>;
