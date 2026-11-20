using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.Aircraft.Commands.AddDocument;

public class AddAircraftDocumentCommandValidator : AbstractValidator<AddAircraftDocumentCommand>
{
    public AddAircraftDocumentCommandValidator()
    {
        RuleFor(x => x.Name).RequiredText("El nombre del documento", 200);
        RuleFor(x => x.Type).RequiredText("El tipo de documento", 100);
        RuleFor(x => x.FileUrl).RequiredText("El archivo", 500);
    }
}
