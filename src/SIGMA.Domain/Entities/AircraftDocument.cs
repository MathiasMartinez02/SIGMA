using SIGMA.Domain.Common;

namespace SIGMA.Domain.Entities;

public class AircraftDocument : AuditableEntity
{
    public Guid AircraftId { get; private set; }
    public Aircraft Aircraft { get; private set; } = null!;
    public string Type { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string FileUrl { get; private set; } = string.Empty;
    public DateTime? ExpiryDate { get; private set; }
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
    public DateTime UploadedAt { get; private set; } = DateTime.UtcNow;

    private AircraftDocument() { }

    public static AircraftDocument Create(Guid aircraftId, string type, string name, string fileUrl, DateTime? expiryDate = null) =>
        new() { AircraftId = aircraftId, Type = type, Name = name, FileUrl = fileUrl, ExpiryDate = expiryDate };
}
