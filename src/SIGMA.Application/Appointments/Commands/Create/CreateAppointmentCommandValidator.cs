using FluentValidation;

namespace SIGMA.Application.Appointments.Commands.Create;

// Valida el comando de creacion de turno: cliente requerido, matricula de referencia si no hay aeronave, fecha futura
public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty().WithMessage("El cliente es requerido.");

        RuleFor(x => x.AircraftRegistrationHint)
            .NotEmpty().WithMessage("La matrícula de referencia es requerida cuando la aeronave no está registrada.")
            .When(x => x.AircraftId is null);

        RuleFor(x => x.ScheduledDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("La fecha del turno debe ser futura.");

        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
