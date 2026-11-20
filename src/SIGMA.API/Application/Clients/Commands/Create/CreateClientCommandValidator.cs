using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.Clients.Commands.Create;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.Name).RequiredText("El nombre del cliente", 200);
        RuleFor(x => x.BusinessName).RequiredText("La razón social", 200);
        RuleFor(x => x.TaxId).Cuit();
        RuleFor(x => x.Email).RequiredEmail();
        RuleFor(x => x.Phone).RequiredText("El teléfono", 30);
        RuleFor(x => x.Address).RequiredText("La dirección", 300);
        RuleFor(x => x.City).RequiredText("La ciudad", 100);
        RuleFor(x => x.Province).RequiredText("La provincia", 100);
        RuleFor(x => x.ContactPerson).OptionalText("El contacto", 150);
        RuleFor(x => x.ContactPhone).OptionalText("El teléfono de contacto", 30);
    }
}
