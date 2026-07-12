namespace SIGMA.Domain.ValueObjects;

public sealed class WorkOrderNumber : IEquatable<WorkOrderNumber>
{
    public string Value { get; }

    private WorkOrderNumber(string value) => Value = value;

    public static WorkOrderNumber Create(int year, int sequence) =>
        new($"OT-{year}-{sequence:D4}");

    public static WorkOrderNumber FromString(string value) => new(value);

    public static implicit operator string(WorkOrderNumber number) => number.Value;

    public bool Equals(WorkOrderNumber? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is WorkOrderNumber n && Equals(n);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}
