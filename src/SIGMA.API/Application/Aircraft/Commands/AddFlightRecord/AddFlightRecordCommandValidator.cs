using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.Aircraft.Commands.AddFlightRecord;

public class AddFlightRecordCommandValidator : AbstractValidator<AddFlightRecordCommand>
{
    public AddFlightRecordCommandValidator()
    {
        RuleFor(x => x.Date).Must(d => d.Date <= DateTime.Today).WithMessage("La fecha del vuelo no puede ser futura.");
        RuleFor(x => x.Duration).GreaterThan(0).WithMessage("La duración del vuelo debe ser mayor a cero.");
        RuleFor(x => x.Landings).GreaterThanOrEqualTo(0).WithMessage("Los aterrizajes no pueden ser negativos.");
        RuleFor(x => x.Pilot).RequiredText("El piloto", 200);
        RuleFor(x => x.Origin).RequiredText("El origen", 100);
        RuleFor(x => x.Destination).RequiredText("El destino", 100);
        RuleFor(x => x.Notes).OptionalText("Las notas", 500);
    }
}
