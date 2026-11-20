using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Users.Commands.ResetPassword;

public record ResetUserPasswordCommand(Guid UserId, string NewPassword) : IRequest<Result>;
