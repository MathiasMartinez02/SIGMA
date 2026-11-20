using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.WorkOrders.Commands.Update;

public class UpdateWorkOrderCommandValidator : AbstractValidator<UpdateWorkOrderCommand>
{
    public UpdateWorkOrderCommandValidator()
    {
        RuleFor(x => x.Description).RequiredText("La descripción", 2000);
        RuleFor(x => x.EstimatedHours).GreaterThan(0).WithMessage("Las horas estimadas deben ser mayores a cero.");
        RuleFor(x => x.Priority).IsInEnum().WithMessage("La prioridad no es válida.");
        RuleFor(x => x.IntakeDate).NotEmpty().WithMessage("La fecha de ingreso es requerida.");
        RuleFor(x => x.EstimatedEndDate).GreaterThanOrEqualTo(x => x.IntakeDate)
            .WithMessage("La fecha estimada de finalización no puede ser anterior a la fecha de ingreso.");
    }
}
