using MediatR;
using SIGMA.Application.Common.Pagination;
using SIGMA.Application.Notifications.DTOs;

namespace SIGMA.Application.Notifications.Queries.GetAll;

// Query para listar, de forma paginada, las notificaciones del usuario autenticado, ordenadas por fecha descendente
public record GetNotificationsQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 10,
    bool? UnreadOnly = null
) : IRequest<PaginatedResult<NotificationDto>>;
