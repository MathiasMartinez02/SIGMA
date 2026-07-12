using SIGMA.Domain.Common;

namespace SIGMA.Domain.Entities;

public class AircraftComponent : AuditableEntity
{
    public Guid AircraftId { get; private set; }
    public Aircraft Aircraft { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string PartNumber { get; private set; } = string.Empty;
    public string SerialNumber { get; private set; } = string.Empty;
    public string Manufacturer { get; private set; } = string.Empty;
    public DateTime InstallDate { get; private set; }
    public decimal InstallHours { get; private set; }
    public decimal? LifeLimitHours { get; private set; }
    public decimal? OverhaulDueHours { get; private set; }
    public string Status { get; private set; } = "activo";

    private AircraftComponent() { }

    public static AircraftComponent Create(
        Guid aircraftId, string name, string partNumber, string serialNumber,
        string manufacturer, DateTime installDate, decimal installHours,
        decimal? lifeLimitHours = null, decimal? overhaulDueHours = null) =>
        new()
        {
            AircraftId = aircraftId, Name = name, PartNumber = partNumber,
            SerialNumber = serialNumber, Manufacturer = manufacturer,
            InstallDate = installDate, InstallHours = installHours,
            LifeLimitHours = lifeLimitHours, OverhaulDueHours = overhaulDueHours
        };
}
