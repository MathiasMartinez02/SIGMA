using SIGMA.Domain.Common;

namespace SIGMA.Domain.Entities;

public class FlightRecord : BaseEntity
{
    public Guid AircraftId { get; private set; }
    public Aircraft Aircraft { get; private set; } = null!;
    public DateTime Date { get; private set; }
    public decimal Duration { get; private set; }
    public int Landings { get; private set; }
    public string Pilot { get; private set; } = string.Empty;
    public string Origin { get; private set; } = string.Empty;
    public string Destination { get; private set; } = string.Empty;
    public string? Notes { get; private set; }

    private FlightRecord() { }

    public static FlightRecord Create(Guid aircraftId, DateTime date, decimal duration,
        int landings, string pilot, string origin, string destination, string? notes = null) =>
        new()
        {
            AircraftId = aircraftId, Date = date, Duration = duration,
            Landings = landings, Pilot = pilot, Origin = origin, Destination = destination, Notes = notes
        };
}
