using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.Inspections.Commands.Complete;

public class CompleteInspectionCommandValidator : AbstractValidator<CompleteInspectionCommand>
{
    public CompleteInspectionCommandValidator()
    {
        RuleFor(x => x.Observations).OptionalText("Las observaciones", 2000);
    }
}
