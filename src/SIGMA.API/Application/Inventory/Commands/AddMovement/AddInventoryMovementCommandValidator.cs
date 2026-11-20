using FluentValidation;
using SIGMA.API.Enums;

namespace SIGMA.API.Inventory.Commands.AddMovement;

public class AddInventoryMovementCommandValidator : AbstractValidator<AddInventoryMovementCommand>
{
    public AddInventoryMovementCommandValidator()
    {
        RuleFor(x => x.Type).IsInEnum().WithMessage("El tipo de movimiento no es válido.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).When(x => x.Type != MovementType.Ajuste)
            .WithMessage("La cantidad del movimiento debe ser mayor a cero.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).When(x => x.Type == MovementType.Ajuste)
            .WithMessage("El stock ajustado no puede ser negativo.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("El motivo del movimiento es requerido.")
            .MaximumLength(500);
    }
}
