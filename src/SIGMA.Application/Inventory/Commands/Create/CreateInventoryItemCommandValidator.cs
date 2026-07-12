using FluentValidation;

namespace SIGMA.Application.Inventory.Commands.Create;

public class CreateInventoryItemCommandValidator : AbstractValidator<CreateInventoryItemCommand>
{
    public CreateInventoryItemCommandValidator()
    {
        RuleFor(x => x.PartNumber)
            .NotEmpty().WithMessage("El número de parte es requerido.")
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es requerida.")
            .MaximumLength(500);

        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo.");

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo.");

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("La unidad de medida es requerida.");

        RuleFor(x => x.Manufacturer)
            .NotEmpty().WithMessage("El fabricante es requerido.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("La ubicación es requerida.");
    }
}
