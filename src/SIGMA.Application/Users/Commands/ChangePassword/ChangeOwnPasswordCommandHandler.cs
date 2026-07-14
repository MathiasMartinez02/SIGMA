using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Users.Commands.ChangePassword;

// Aplica el cambio de contrasena propio: verifica la contrasena actual contra el hash guardado antes de reemplazarla
public class ChangeOwnPasswordCommandHandler : IRequestHandler<ChangeOwnPasswordCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public ChangeOwnPasswordCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ChangeOwnPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

        if (user is null)
            return Result.Failure("El usuario no fue encontrado.");

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            return Result.Failure("La contraseña actual es incorrecta.");

        user.UpdatePasswordHash(_passwordHasher.Hash(request.NewPassword));
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
