using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Users.DTOs;

namespace SIGMA.Application.Users.Queries.GetById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUserByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);

        if (user is null)
            return Result<UserDto>.Failure("El usuario no fue encontrado.");

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
