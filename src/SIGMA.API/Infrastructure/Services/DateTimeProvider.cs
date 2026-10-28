using SIGMA.API.Common.Interfaces;

namespace SIGMA.API.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public int CurrentYear => DateTime.UtcNow.Year;
}
