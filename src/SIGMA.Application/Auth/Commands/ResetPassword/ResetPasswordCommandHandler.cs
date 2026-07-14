using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Auth.Commands.ResetPassword;

// Busca al usuario por el token de reseteo vigente (no vencido) y aplica la nueva contrasena, consumiendo el token
public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTime;

    public ResetPasswordCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IDateTimeProvider dateTime)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token && !u.IsDeleted, cancellationToken);

        if (user is null || user.PasswordResetTokenExpiry is null || user.PasswordResetTokenExpiry < _dateTime.UtcNow)
            return Result.Failure("El token de reseteo no es valido o expiro. Solicite uno nuevo.");

        user.ResetPassword(_passwordHasher.Hash(request.NewPassword));
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
