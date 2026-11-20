using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Auth;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.Users.Commands.ResetPassword;

public class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public ResetUserPasswordCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    // Contrasena nueva asignada por el gerente: cierra sesiones y resuelve las solicitudes pendientes
    public async Task<Result> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (user is null)
            return Result.Failure("El usuario no fue encontrado.");

        user.UpdatePasswordHash(_passwordHasher.Hash(request.NewPassword));
        await RefreshTokenRevoker.RevokeAllAsync(_context, user.Id, cancellationToken);

        var pendingRequests = await _context.Notifications
            .Where(n => n.Type == NotificationType.SolicitudCambioPassword && n.RelatedEntityId == user.Id && !n.IsRead)
            .ToListAsync(cancellationToken);
        foreach (var notification in pendingRequests)
            notification.MarkAsRead();

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
