using FluentValidation;
using SIGMA.API.Common.Validation;

namespace SIGMA.API.Appointments.Commands.Update;

public class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
{
    public UpdateAppointmentCommandValidator()
    {
        RuleFor(x => x.AircraftRegistrationHint)
            .NotEmpty().WithMessage("La matrícula de referencia es requerida cuando la aeronave no está registrada.")
            .When(x => x.AircraftId is null);
        RuleFor(x => x.AircraftRegistrationHint).OptionalText("La matrícula de referencia", 20);
        RuleFor(x => x.RequestedType).IsInEnum().WithMessage("El tipo de trabajo no es válido.");
        RuleFor(x => x.ScheduledDate).GreaterThan(_ => DateTime.Now).WithMessage("La fecha y hora del turno deben ser futuras.");
        RuleFor(x => x.Notes).OptionalText("Las notas", 1000);
    }
}
