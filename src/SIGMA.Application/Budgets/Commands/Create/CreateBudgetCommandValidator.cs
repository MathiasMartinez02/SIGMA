using FluentValidation;

namespace SIGMA.Application.Budgets.Commands.Create;

// Valida el comando de creacion de presupuesto: cliente y descripcion requeridos, monto positivo
public class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetCommandValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty().WithMessage("El cliente es requerido.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción del trabajo presupuestado es requerida.")
            .MaximumLength(2000);

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a cero.");

        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
