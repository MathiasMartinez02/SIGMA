using FluentValidation;

namespace SIGMA.Application.Clients.Commands.Create;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del cliente es requerido.")
            .MaximumLength(200);

        RuleFor(x => x.TaxId)
            .NotEmpty().WithMessage("El CUIT es requerido.")
            .Matches(@"^\d{2}-\d{8}-\d{1}$").WithMessage("El CUIT debe tener formato argentino XX-XXXXXXXX-X.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");

        RuleFor(x => x.BusinessName)
            .NotEmpty().WithMessage("La razón social es requerida.")
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("El teléfono es requerido.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("La dirección es requerida.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("La ciudad es requerida.");

        RuleFor(x => x.Province)
            .NotEmpty().WithMessage("La provincia es requerida.");
    }
}
