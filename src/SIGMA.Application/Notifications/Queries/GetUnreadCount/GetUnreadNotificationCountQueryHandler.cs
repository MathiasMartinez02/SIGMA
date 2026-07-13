using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;

namespace SIGMA.Application.Notifications.Queries.GetUnreadCount;

// Cuenta las notificaciones no leidas del usuario sin traer el listado completo
public class GetUnreadNotificationCountQueryHandler : IRequestHandler<GetUnreadNotificationCountQuery, int>
{
    private readonly IApplicationDbContext _context;

    public GetUnreadNotificationCountQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken) =>
        await _context.Notifications.CountAsync(n => n.UserId == request.UserId && !n.IsRead, cancellationToken);
}
