using FluentValidation;

namespace SIGMA.Application.Aircraft.Commands.Create;

public class CreateAircraftCommandValidator : AbstractValidator<CreateAircraftCommand>
{
    public CreateAircraftCommandValidator()
    {
        RuleFor(x => x.Registration)
            .NotEmpty().WithMessage("La matrícula es requerida.")
            .Matches(@"^[A-Z]{2}-[A-Z]{3,4}$").WithMessage("La matrícula debe tener formato XX-XXXX (ej: LV-ABC).");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("El modelo es requerido.")
            .MaximumLength(100);

        RuleFor(x => x.Manufacturer)
            .NotEmpty().WithMessage("El fabricante es requerido.")
            .MaximumLength(100);

        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("El cliente es requerido.");

        RuleFor(x => x.Year)
            .InclusiveBetween(1900, DateTime.UtcNow.Year + 5)
            .WithMessage($"El año debe estar entre 1900 y {DateTime.UtcNow.Year + 5}.");

        RuleFor(x => x.TotalFlightHours)
            .GreaterThanOrEqualTo(0).WithMessage("Las horas de vuelo no pueden ser negativas.");

        RuleFor(x => x.TotalLandings)
            .GreaterThanOrEqualTo(0).WithMessage("Los aterrizajes no pueden ser negativos.");

        RuleFor(x => x.CertificateExpiry)
            .GreaterThan(DateTime.UtcNow).WithMessage("La fecha de vencimiento del certificado debe ser futura.");

        RuleFor(x => x.NextInspectionHours)
            .GreaterThanOrEqualTo(0).WithMessage("Las horas de próxima inspección no pueden ser negativas.");
    }
}
