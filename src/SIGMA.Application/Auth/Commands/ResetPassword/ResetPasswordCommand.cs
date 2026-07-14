using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Auth.Commands.ResetPassword;

// Segundo paso del flujo de recuperacion: aplica la nueva contrasena usando el token generado por ForgotPasswordCommand
public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<Result>;
