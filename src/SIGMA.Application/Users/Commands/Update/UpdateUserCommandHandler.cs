using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Users.Commands.Update;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);

        if (user is null)
            return Result.Failure("El usuario no fue encontrado.");

        user.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.Phone,
            request.LicenseNumber,
            request.LicenseExpiry,
            request.AvatarUrl);

        if (user.Role != request.Role)
            user.ChangeRole(request.Role);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
