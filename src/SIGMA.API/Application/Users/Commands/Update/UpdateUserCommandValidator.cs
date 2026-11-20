using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.Users.Commands.Update;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.FirstName).RequiredText("El nombre", 100);
        RuleFor(x => x.LastName).RequiredText("El apellido", 100);
        RuleFor(x => x.Role).IsInEnum().WithMessage("El rol no es válido.");
        RuleFor(x => x.LicenseNumber).OptionalText("El número de licencia", 50);
        RuleFor(x => x.Phone).OptionalText("El teléfono", 30);
        RuleFor(x => x.AvatarUrl).OptionalText("La URL del avatar", 500);
    }
}
