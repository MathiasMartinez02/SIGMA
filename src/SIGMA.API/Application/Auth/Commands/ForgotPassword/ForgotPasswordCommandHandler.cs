using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Entities;
using SIGMA.API.Enums;

namespace SIGMA.API.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public ForgotPasswordCommandHandler(IApplicationDbContext context) => _context = context;

    // Notifica a los gerentes la solicitud; responde igual exista o no el email
    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive && !u.IsDeleted, cancellationToken);

        if (user is null) return Result.Success();

        var managers = await _context.Users
            .Where(u => u.Role == UserRole.Gerente && u.IsActive && !u.IsDeleted && u.Id != user.Id)
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        var alreadyNotified = await _context.Notifications
            .Where(n => n.Type == NotificationType.SolicitudCambioPassword && n.RelatedEntityId == user.Id && !n.IsRead)
            .Select(n => n.UserId)
            .ToListAsync(cancellationToken);

        foreach (var managerId in managers.Except(alreadyNotified))
        {
            _context.Notifications.Add(Notification.Create(
                managerId,
                NotificationType.SolicitudCambioPassword,
                "Solicitud de cambio de contraseña",
                $"{user.FullName} ({user.Email}) pidió restablecer su contraseña. Asignale una nueva desde Usuarios.",
                user.Id));
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
