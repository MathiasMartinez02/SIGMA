using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Auth.Commands.ForgotPassword;

// Genera y persiste un token de reseteo de contrasena de un solo uso, valido 1 hora
public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<ForgotPasswordResultDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTime;

    public ForgotPasswordCommandHandler(IApplicationDbContext context, IDateTimeProvider dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    public async Task<Result<ForgotPasswordResultDto>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant() && !u.IsDeleted, cancellationToken);

        if (user is null)
            return Result<ForgotPasswordResultDto>.Failure("No existe un usuario con ese email.");

        var token = Guid.NewGuid().ToString("N");
        var expiry = _dateTime.UtcNow.AddHours(1);
        user.SetPasswordResetToken(token, expiry);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<ForgotPasswordResultDto>.Success(new ForgotPasswordResultDto
        {
            ResetToken = token,
            ExpiresAt = expiry
        });
    }
}
