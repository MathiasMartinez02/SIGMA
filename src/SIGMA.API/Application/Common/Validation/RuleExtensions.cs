using FluentValidation;

namespace SIGMA.API.Common.Validation;

public static class RuleExtensions
{
    // Texto obligatorio con largo maximo
    public static IRuleBuilderOptions<T, string> RequiredText<T>(this IRuleBuilder<T, string> rule, string field, int maxLength) =>
        rule.NotEmpty().WithMessage($"Completá {char.ToLowerInvariant(field[0])}{field[1..]}.")
            .MaximumLength(maxLength).WithMessage($"{field} no puede superar los {maxLength} caracteres.");

    // Texto opcional con largo maximo
    public static IRuleBuilderOptions<T, string?> OptionalText<T>(this IRuleBuilder<T, string?> rule, string field, int maxLength) =>
        rule.MaximumLength(maxLength).WithMessage($"{field} no puede superar los {maxLength} caracteres.");

    // CUIT argentino XX-XXXXXXXX-X
    public static IRuleBuilderOptions<T, string> Cuit<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("El CUIT es requerido.")
            .Matches(@"^\d{2}-\d{8}-\d$").WithMessage("El CUIT debe tener formato argentino XX-XXXXXXXX-X.");

    // Email obligatorio con formato y largo valido
    public static IRuleBuilderOptions<T, string> RequiredEmail<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("El email es requerido.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .MaximumLength(256).WithMessage("El email no puede superar los 256 caracteres.");

    // Fecha de negocio desde hoy en adelante (sin hora)
    public static IRuleBuilderOptions<T, DateTime> TodayOrLater<T>(this IRuleBuilder<T, DateTime> rule, string message) =>
        rule.Must(date => date.Date >= DateTime.Today).WithMessage(message);
}
