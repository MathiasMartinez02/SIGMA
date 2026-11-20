using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.Inventory.Commands.Update;

public class UpdateInventoryItemCommandValidator : AbstractValidator<UpdateInventoryItemCommand>
{
    public UpdateInventoryItemCommandValidator()
    {
        RuleFor(x => x.Description).RequiredText("La descripción", 500);
        RuleFor(x => x.Location).RequiredText("La ubicación", 100);
        RuleFor(x => x.MinimumStock).GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo.");
        RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo.");
        RuleFor(x => x.MaximumStock)
            .GreaterThan(x => x.MinimumStock)
            .WithMessage("El stock máximo debe ser mayor al stock mínimo.")
            .When(x => x.MaximumStock.HasValue);
        RuleFor(x => x.CertificateNumber).OptionalText("El número de certificado", 100);
    }
}
