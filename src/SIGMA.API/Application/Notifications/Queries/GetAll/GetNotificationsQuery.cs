using MediatR;
using SIGMA.API.Common.Pagination;
using SIGMA.API.Notifications.DTOs;

namespace SIGMA.API.Notifications.Queries.GetAll;

// Query para listar, de forma paginada, las notificaciones del usuario autenticado, ordenadas por fecha descendente
public record GetNotificationsQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 10,
    bool? UnreadOnly = null
) : IRequest<PaginatedResult<NotificationDto>>;
