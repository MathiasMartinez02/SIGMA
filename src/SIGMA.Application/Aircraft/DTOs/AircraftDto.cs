using SIGMA.Domain.Enums;

namespace SIGMA.Application.Aircraft.DTOs;

public class AircraftDto
{
    public Guid Id { get; init; }
    public string Registration { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public string Manufacturer { get; init; } = string.Empty;
    public int Year { get; init; }
    public AircraftCategory Category { get; init; }
    public AircraftStatus Status { get; init; }
    public string SerialNumber { get; init; } = string.Empty;
    public string EngineModel { get; init; } = string.Empty;
    public string EngineSerialNumber { get; init; } = string.Empty;
    public decimal TotalFlightHours { get; init; }
    public decimal TotalLandings { get; init; }
    public DateTime? LastInspectionDate { get; init; }
    public DateTime? NextInspectionDue { get; init; }
    public decimal NextInspectionHours { get; init; }
    public DateTime CertificateExpiry { get; init; }
    public bool IsCertificateExpiringSoon { get; init; }
    public Guid ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public class AircraftDetailDto : AircraftDto
{
    public IList<AircraftDocumentDto> Documents { get; init; } = [];
    public IList<FlightRecordDto> FlightHistory { get; init; } = [];
    public IList<AircraftComponentDto> Components { get; init; } = [];
}

public class AircraftDocumentDto
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string FileUrl { get; init; } = string.Empty;
    public DateTime? ExpiryDate { get; init; }
    public bool IsExpired { get; init; }
    public DateTime UploadedAt { get; init; }
}

public class FlightRecordDto
{
    public Guid Id { get; init; }
    public DateTime Date { get; init; }
    public decimal Duration { get; init; }
    public int Landings { get; init; }
    public string Pilot { get; init; } = string.Empty;
    public string Origin { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public string? Notes { get; init; }
}

public class AircraftComponentDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string PartNumber { get; init; } = string.Empty;
    public string SerialNumber { get; init; } = string.Empty;
    public string Manufacturer { get; init; } = string.Empty;
    public DateTime InstallDate { get; init; }
    public decimal InstallHours { get; init; }
    public decimal? LifeLimitHours { get; init; }
    public decimal? OverhaulDueHours { get; init; }
    public string Status { get; init; } = string.Empty;
}
