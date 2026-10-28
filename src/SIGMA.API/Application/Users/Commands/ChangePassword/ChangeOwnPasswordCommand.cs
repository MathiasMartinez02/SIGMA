using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Users.Commands.ChangePassword;

// Cambio de contrasena propio: el usuario autenticado envia su contrasena actual y la nueva
public record ChangeOwnPasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest<Result>;
