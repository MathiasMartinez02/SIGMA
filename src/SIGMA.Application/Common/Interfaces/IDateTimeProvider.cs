namespace SIGMA.Application.Common.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    int CurrentYear { get; }
}
