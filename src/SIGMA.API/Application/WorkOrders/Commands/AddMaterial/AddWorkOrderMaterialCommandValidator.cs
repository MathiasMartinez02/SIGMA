using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.WorkOrders.Commands.AddMaterial;

public class AddWorkOrderMaterialCommandValidator : AbstractValidator<AddWorkOrderMaterialCommand>
{
    public AddWorkOrderMaterialCommandValidator()
    {
        RuleFor(x => x.PartNumber).RequiredText("El número de parte", 100);
        RuleFor(x => x.Description).RequiredText("La descripción", 500);
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.");
        RuleFor(x => x.Unit).RequiredText("La unidad", 30);
    }
}
