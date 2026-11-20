using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.WorkOrders.Commands.AddTask;

public class AddWorkOrderTaskCommandValidator : AbstractValidator<AddWorkOrderTaskCommand>
{
    public AddWorkOrderTaskCommandValidator()
    {
        RuleFor(x => x.Title).RequiredText("El título", 300);
        RuleFor(x => x.Description).RequiredText("La descripción", 2000);
        RuleFor(x => x.EstimatedHours).GreaterThan(0).WithMessage("Las horas estimadas deben ser mayores a cero.");
    }
}
