using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.WorkOrders.Commands.AddDocument;

public class AddWorkOrderDocumentCommandValidator : AbstractValidator<AddWorkOrderDocumentCommand>
{
    public AddWorkOrderDocumentCommandValidator()
    {
        RuleFor(x => x.Name).RequiredText("El nombre del documento", 200);
        RuleFor(x => x.Type).RequiredText("El tipo de documento", 100);
        RuleFor(x => x.FileUrl).RequiredText("El archivo", 500);
    }
}
