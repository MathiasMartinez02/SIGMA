using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.Inspections.Commands.Approve;

public class ApproveInspectionCommandValidator : AbstractValidator<ApproveInspectionCommand>
{
    public ApproveInspectionCommandValidator()
    {
        RuleFor(x => x.Observations).OptionalText("Las observaciones", 2000);
    }
}
