using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Users.Commands.UpdateStatus;

public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateUserStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
    {
        if (!request.IsActive && _currentUser.UserId.HasValue && _currentUser.UserId.Value == request.Id)
            return Result.Failure("No puedes desactivar tu propia cuenta.");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);

        if (user is null)
            return Result.Failure("El usuario no fue encontrado.");

        user.SetActive(request.IsActive);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
