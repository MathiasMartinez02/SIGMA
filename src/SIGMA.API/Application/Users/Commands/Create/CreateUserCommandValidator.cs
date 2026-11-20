using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.Users.Commands.Create;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).RequiredEmail();
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .MaximumLength(100);
        RuleFor(x => x.FirstName).RequiredText("El nombre", 100);
        RuleFor(x => x.LastName).RequiredText("El apellido", 100);
        RuleFor(x => x.Role).IsInEnum().WithMessage("El rol no es válido.");
        RuleFor(x => x.LicenseNumber).OptionalText("El número de licencia", 50);
        RuleFor(x => x.Phone).OptionalText("El teléfono", 30);
    }
}
