using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.Inspections.Commands.UpdateChecklistItem;

public class UpdateChecklistItemCommandValidator : AbstractValidator<UpdateChecklistItemCommand>
{
    public UpdateChecklistItemCommandValidator()
    {
        RuleFor(x => x.Status).IsInEnum().WithMessage("El resultado del ítem no es válido.");
        RuleFor(x => x.Observations).OptionalText("Las observaciones", 1000);
    }
}
