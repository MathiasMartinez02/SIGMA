using FluentValidation;

namespace SIGMA.Application.WorkOrders.Commands.Create;

public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderCommandValidator()
    {
        RuleFor(x => x.AircraftId).NotEmpty().WithMessage("La aeronave es requerida.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("La descripción es requerida.")
            .MaximumLength(2000);
        RuleFor(x => x.EstimatedHours).GreaterThan(0).WithMessage("Las horas estimadas deben ser mayores a cero.");
        RuleFor(x => x.IntakeDate).NotEmpty().WithMessage("La fecha de ingreso es requerida.");
        RuleFor(x => x.EstimatedEndDate).GreaterThan(DateTime.UtcNow)
            .WithMessage("La fecha estimada de finalización debe ser futura.");
        RuleFor(x => x.EstimatedEndDate).GreaterThanOrEqualTo(x => x.IntakeDate)
            .WithMessage("La fecha estimada de finalización no puede ser anterior a la fecha de ingreso.");
    }
}
