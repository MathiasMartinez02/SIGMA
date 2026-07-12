using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Users.DTOs;
using SIGMA.Domain.Entities;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Users.Commands.Create;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _currentUser = currentUser;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (request.Role == UserRole.Gerente)
        {
            if (_currentUser.Role != nameof(UserRole.Gerente))
                return Result<UserDto>.Failure("Solo un gerente puede crear otros usuarios con rol de gerente.");
        }

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email.ToLowerInvariant() && !u.IsDeleted, cancellationToken);

        if (emailExists)
            return Result<UserDto>.Failure($"Ya existe un usuario con el email '{request.Email}'.");

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = User.Create(
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName,
            request.Role,
            request.LicenseNumber,
            request.Phone);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            Role = user.Role,
            LicenseNumber = user.LicenseNumber,
            LicenseExpiry = user.LicenseExpiry,
            Phone = user.Phone,
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt
        };

        return Result<UserDto>.Success(dto);
    }
}
