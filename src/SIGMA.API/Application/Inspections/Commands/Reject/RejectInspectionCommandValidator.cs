using FluentValidation;

namespace SIGMA.API.Inspections.Commands.Reject;

public class RejectInspectionCommandValidator : AbstractValidator<RejectInspectionCommand>
{
    public RejectInspectionCommandValidator()
    {
        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("El motivo de rechazo es requerido.")
            .MaximumLength(1000).WithMessage("El motivo de rechazo no puede superar los 1000 caracteres.");
    }
}
