using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;
