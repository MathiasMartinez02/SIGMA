using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Notifications.Commands.MarkAllAsRead;

// Marca como leidas todas las notificaciones no leidas del usuario actual
public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public MarkAllNotificationsAsReadCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        var unread = await _context.Notifications
            .Where(n => n.UserId == request.UserId && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in unread)
            notification.MarkAsRead();

        if (unread.Count > 0)
            await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
