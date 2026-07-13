using FluentValidation;

namespace SIGMA.Application.Inspections.Commands.Create;

// Valida que la orden de trabajo y la fecha programada estén presentes al crear una inspección
public class CreateInspectionCommandValidator : AbstractValidator<CreateInspectionCommand>
{
    public CreateInspectionCommandValidator()
    {
        RuleFor(x => x.WorkOrderId)
            .NotEmpty().WithMessage("La orden de trabajo es requerida.");

        RuleFor(x => x.ScheduledDate)
            .NotEmpty().WithMessage("La fecha programada es requerida.");
    }
}
