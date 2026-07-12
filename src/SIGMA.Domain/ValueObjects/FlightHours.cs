using SIGMA.Domain.Exceptions;

namespace SIGMA.Domain.ValueObjects;

public sealed class FlightHours : IEquatable<FlightHours>, IComparable<FlightHours>
{
    public decimal Value { get; }

    private FlightHours(decimal value) => Value = value;

    public static FlightHours Create(decimal value)
    {
        if (value < 0)
            throw new DomainException("Las horas de vuelo no pueden ser negativas.");
        return new FlightHours(Math.Round(value, 1));
    }

    public static FlightHours Zero => new(0);

    public FlightHours Add(FlightHours other) => new(Value + other.Value);

    public static implicit operator decimal(FlightHours hours) => hours.Value;

    public bool Equals(FlightHours? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is FlightHours h && Equals(h);
    public override int GetHashCode() => Value.GetHashCode();
    public int CompareTo(FlightHours? other) => Value.CompareTo(other?.Value ?? 0);
    public override string ToString() => $"{Value:F1}h";
}
