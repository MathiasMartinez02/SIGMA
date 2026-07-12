using System.Text.RegularExpressions;
using SIGMA.Domain.Exceptions;

namespace SIGMA.Domain.ValueObjects;

public sealed class Registration : IEquatable<Registration>
{
    private static readonly Regex Pattern = new(@"^[A-Z]{2}-[A-Z]{3,4}$", RegexOptions.Compiled);

    public string Value { get; }

    private Registration(string value) => Value = value;

    public static Registration Create(string value)
    {
        var normalized = value?.Trim().ToUpperInvariant() ?? string.Empty;
        if (!Pattern.IsMatch(normalized))
            throw new DomainException($"La matrícula '{value}' no es válida. Formato requerido: XX-XXX o XX-XXXX.");
        return new Registration(normalized);
    }

    public static implicit operator string(Registration registration) => registration.Value;

    public bool Equals(Registration? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is Registration r && Equals(r);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}
