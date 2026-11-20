using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.WorkOrders.Commands.UpdateTaskStatus;

public class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
{
    public UpdateTaskStatusCommandValidator()
    {
        RuleFor(x => x.Status).IsInEnum().WithMessage("El estado de la tarea no es válido.");
        RuleFor(x => x.Observations).OptionalText("Las observaciones", 1000);
        RuleFor(x => x.ActualHours).GreaterThan(0).When(x => x.ActualHours.HasValue)
            .WithMessage("Las horas reales deben ser mayores a cero.");
    }
}
