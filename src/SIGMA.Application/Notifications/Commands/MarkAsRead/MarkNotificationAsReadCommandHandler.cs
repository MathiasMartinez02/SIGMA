using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Notifications.Commands.MarkAsRead;

// Busca la notificacion del usuario actual y la marca como leida
public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public MarkNotificationAsReadCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId, cancellationToken);

        if (notification is null)
            return Result.Failure("La notificacion no fue encontrada.");

        if (notification.UserId != request.UserId)
            return Result.Failure("La notificacion no pertenece al usuario actual.");

        notification.MarkAsRead();
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
