using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.Aircraft.Commands.Update;

public class UpdateAircraftCommandValidator : AbstractValidator<UpdateAircraftCommand>
{
    public UpdateAircraftCommandValidator()
    {
        RuleFor(x => x.Model).RequiredText("El modelo", 100);
        RuleFor(x => x.Manufacturer).RequiredText("El fabricante", 100);
        RuleFor(x => x.SerialNumber).RequiredText("El número de serie", 100);
        RuleFor(x => x.EngineModel).RequiredText("El modelo de motor", 100);
        RuleFor(x => x.EngineSerialNumber).RequiredText("El número de serie del motor", 100);
        RuleFor(x => x.Year)
            .InclusiveBetween(1900, DateTime.Today.Year + 5)
            .WithMessage($"El año debe estar entre 1900 y {DateTime.Today.Year + 5}.");
        RuleFor(x => x.TotalFlightHours).GreaterThanOrEqualTo(0).WithMessage("Las horas de vuelo no pueden ser negativas.");
        RuleFor(x => x.TotalLandings).GreaterThanOrEqualTo(0).WithMessage("Los aterrizajes no pueden ser negativos.");
        RuleFor(x => x.NextInspectionHours).GreaterThanOrEqualTo(0).WithMessage("Las horas de próxima inspección no pueden ser negativas.");

        RuleFor(x => x.CertificateExpiry).NotEmpty().WithMessage("El vencimiento del certificado es requerido.");
    }
}
