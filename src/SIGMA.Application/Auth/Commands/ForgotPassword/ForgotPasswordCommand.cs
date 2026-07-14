using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Auth.Commands.ForgotPassword;

// Fase 7 (MVP sin envio de email real): genera un token de reseteo de un solo uso con expiracion.
// El token se devuelve en la respuesta en lugar de enviarse por correo porque el proyecto no tiene
// infraestructura de email configurada (decision de alcance documentada en PLAN_ANALISIS_FUNCIONAL.md).
public record ForgotPasswordCommand(string Email) : IRequest<Result<ForgotPasswordResultDto>>;

public class ForgotPasswordResultDto
{
    // Token de un solo uso, valido por 1 hora. En un flujo real este valor viajaria por email, nunca en la respuesta HTTP.
    public string? ResetToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
