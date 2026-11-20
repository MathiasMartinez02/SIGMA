using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.Inventory.Commands.Create;

public class CreateInventoryItemCommandValidator : AbstractValidator<CreateInventoryItemCommand>
{
    public CreateInventoryItemCommandValidator()
    {
        RuleFor(x => x.PartNumber).RequiredText("El número de parte", 100);
        RuleFor(x => x.Manufacturer).RequiredText("El fabricante", 150);
        RuleFor(x => x.Unit).RequiredText("La unidad de medida", 30);
        RuleFor(x => x.CertificateNumber).OptionalText("El número de certificado", 100);
        RuleFor(x => x.CertificateNumber)
            .NotEmpty().When(x => x.CertificationRequired)
            .WithMessage("El ítem requiere certificación: indicá el número de certificado.");

        RuleFor(x => x.Description).RequiredText("La descripción", 500);
        RuleFor(x => x.Location).RequiredText("La ubicación", 100);
        RuleFor(x => x.MinimumStock).GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo.");
        RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo.");
        RuleFor(x => x.MaximumStock)
            .GreaterThan(x => x.MinimumStock)
            .WithMessage("El stock máximo debe ser mayor al stock mínimo.")
            .When(x => x.MaximumStock.HasValue);
    }
}
