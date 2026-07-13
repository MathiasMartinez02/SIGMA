using FluentValidation;

namespace SIGMA.Application.Inventory.Commands.Update;

// Valida el comando de actualizacion de item de inventario, incluyendo el nuevo campo MaximumStock
public class UpdateInventoryItemCommandValidator : AbstractValidator<UpdateInventoryItemCommand>
{
    public UpdateInventoryItemCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es requerida.")
            .MaximumLength(500);

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("La ubicación es requerida.");

        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo.");

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo.");

        // Si se informa un stock maximo, debe ser mayor al stock minimo configurado
        RuleFor(x => x.MaximumStock)
            .GreaterThan(x => x.MinimumStock)
            .WithMessage("El stock máximo debe ser mayor al stock mínimo.")
            .When(x => x.MaximumStock.HasValue);
    }
}
