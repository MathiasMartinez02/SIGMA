using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.Aircraft.Commands.AddComponent;

public class AddAircraftComponentCommandValidator : AbstractValidator<AddAircraftComponentCommand>
{
    public AddAircraftComponentCommandValidator()
    {
        RuleFor(x => x.Name).RequiredText("El nombre del componente", 200);
        RuleFor(x => x.PartNumber).RequiredText("El número de parte", 100);
        RuleFor(x => x.SerialNumber).RequiredText("El número de serie", 100);
        RuleFor(x => x.Manufacturer).RequiredText("El fabricante", 150);
        RuleFor(x => x.InstallHours).GreaterThanOrEqualTo(0).WithMessage("Las horas de instalación no pueden ser negativas.");
        RuleFor(x => x.LifeLimitHours).GreaterThan(0).When(x => x.LifeLimitHours.HasValue)
            .WithMessage("El límite de vida debe ser mayor a cero.");
        RuleFor(x => x.OverhaulDueHours).GreaterThan(0).When(x => x.OverhaulDueHours.HasValue)
            .WithMessage("Las horas de overhaul deben ser mayores a cero.");
    }
}
