using SIGMA.Application.Common.Interfaces;

namespace SIGMA.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public int CurrentYear => DateTime.UtcNow.Year;
}
