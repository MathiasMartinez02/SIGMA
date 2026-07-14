using FluentValidation;

namespace SIGMA.Application.TechnicalDocuments.Commands.Create;

public class CreateTechnicalDocumentCommandValidator : AbstractValidator<CreateTechnicalDocumentCommand>
{
    public CreateTechnicalDocumentCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El titulo del documento es requerido.")
            .MaximumLength(200);

        RuleFor(x => x.FileUrl)
            .NotEmpty().WithMessage("El archivo/URL del documento es requerido.")
            .MaximumLength(500);

        RuleFor(x => x.ReferenceCode)
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.ExpiryDate)
            .GreaterThanOrEqualTo(x => x.IssueDate)
            .WithMessage("La fecha de vencimiento no puede ser anterior a la fecha de emision.")
            .When(x => x.ExpiryDate.HasValue && x.IssueDate.HasValue);
    }
}
