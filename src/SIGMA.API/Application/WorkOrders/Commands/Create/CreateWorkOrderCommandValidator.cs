using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.WorkOrders.Commands.Create;

public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderCommandValidator()
    {
        RuleFor(x => x.AircraftId).NotEmpty().WithMessage("La aeronave es requerida.");
        RuleFor(x => x.Description).RequiredText("La descripción", 2000);
        RuleFor(x => x.EstimatedHours).GreaterThan(0).WithMessage("Las horas estimadas deben ser mayores a cero.");
        RuleFor(x => x.IntakeDate).NotEmpty().WithMessage("La fecha de ingreso es requerida.");
        RuleFor(x => x.EstimatedEndDate).TodayOrLater("La fecha estimada de finalización no puede ser pasada.");
        RuleFor(x => x.EstimatedEndDate).GreaterThanOrEqualTo(x => x.IntakeDate)
            .WithMessage("La fecha estimada de finalización no puede ser anterior a la fecha de ingreso.");
    }
}
