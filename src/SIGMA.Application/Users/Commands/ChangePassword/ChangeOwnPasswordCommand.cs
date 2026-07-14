using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Users.Commands.ChangePassword;

// Cambio de contrasena propio: el usuario autenticado envia su contrasena actual y la nueva
public record ChangeOwnPasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest<Result>;
