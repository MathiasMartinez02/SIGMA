using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Pagination;
using SIGMA.Application.Notifications.DTOs;

namespace SIGMA.Application.Notifications.Queries.GetAll;

// Maneja la consulta paginada de notificaciones del usuario, con filtro opcional de solo no leidas
public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, PaginatedResult<NotificationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetNotificationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Notifications
            .Where(n => n.UserId == request.UserId)
            .AsQueryable();

        if (request.UnreadOnly == true)
            query = query.Where(n => !n.IsRead);

        query = query.OrderByDescending(n => n.CreatedAt);

        var mapped = query.Select(n => new NotificationDto
        {
            Id = n.Id,
            Type = n.Type,
            Title = n.Title,
            Message = n.Message,
            RelatedEntityId = n.RelatedEntityId,
            IsRead = n.IsRead,
            ReadAt = n.ReadAt,
            CreatedAt = n.CreatedAt
        });

        return await mapped.ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}
