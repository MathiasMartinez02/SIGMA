using SIGMA.Domain.Common;
using SIGMA.Domain.Enums;
using SIGMA.Domain.Exceptions;
using SIGMA.Domain.ValueObjects;

namespace SIGMA.Domain.Entities;

public class Aircraft : AuditableEntity
{
    private string _registration = string.Empty;

    public string Registration
    {
        get => _registration;
        private set => _registration = value;
    }

    public string Model { get; private set; } = string.Empty;
    public string Manufacturer { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public AircraftCategory Category { get; private set; }
    public AircraftStatus Status { get; private set; } = AircraftStatus.Operativa;
    public string SerialNumber { get; private set; } = string.Empty;
    public string EngineModel { get; private set; } = string.Empty;
    public string EngineSerialNumber { get; private set; } = string.Empty;
    public decimal TotalFlightHours { get; private set; }
    public decimal TotalLandings { get; private set; }
    public DateTime? LastInspectionDate { get; private set; }
    public DateTime? NextInspectionDue { get; private set; }
    public decimal NextInspectionHours { get; private set; }
    public DateTime CertificateExpiry { get; private set; }
    public Guid ClientId { get; private set; }
    public Client Client { get; private set; } = null!;

    public ICollection<AircraftDocument> Documents { get; private set; } = [];
    public ICollection<FlightRecord> FlightRecords { get; private set; } = [];
    public ICollection<AircraftComponent> Components { get; private set; } = [];
    public ICollection<WorkOrder> WorkOrders { get; private set; } = [];

    private Aircraft() { }

    public static Aircraft Create(
        string registration, string model, string manufacturer, int year,
        AircraftCategory category, string serialNumber, string engineModel,
        string engineSerialNumber, decimal totalFlightHours, decimal totalLandings,
        DateTime? lastInspectionDate, DateTime? nextInspectionDue,
        decimal nextInspectionHours, DateTime certificateExpiry, Guid clientId)
    {
        var reg = ValueObjects.Registration.Create(registration);

        return new Aircraft
        {
            Registration = reg.Value,
            Model = model.Trim(),
            Manufacturer = manufacturer.Trim(),
            Year = year,
            Category = category,
            SerialNumber = serialNumber.Trim(),
            EngineModel = engineModel.Trim(),
            EngineSerialNumber = engineSerialNumber.Trim(),
            TotalFlightHours = totalFlightHours,
            TotalLandings = totalLandings,
            LastInspectionDate = lastInspectionDate,
            NextInspectionDue = nextInspectionDue,
            NextInspectionHours = nextInspectionHours,
            CertificateExpiry = certificateExpiry,
            ClientId = clientId
        };
    }

    public void UpdateStatus(AircraftStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFlightHours(decimal newHours, decimal newLandings)
    {
        if (newHours < TotalFlightHours)
            throw new DomainException("Las horas de vuelo no pueden disminuir.");
        TotalFlightHours = newHours;
        TotalLandings = newLandings;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string model, string manufacturer, int year, AircraftCategory category,
        string serialNumber, string engineModel, string engineSerialNumber,
        decimal totalFlightHours, decimal totalLandings,
        DateTime? lastInspectionDate, DateTime? nextInspectionDue,
        decimal nextInspectionHours, DateTime certificateExpiry)
    {
        Model = model.Trim();
        Manufacturer = manufacturer.Trim();
        Year = year;
        Category = category;
        SerialNumber = serialNumber.Trim();
        EngineModel = engineModel.Trim();
        EngineSerialNumber = engineSerialNumber.Trim();
        TotalFlightHours = totalFlightHours;
        TotalLandings = totalLandings;
        LastInspectionDate = lastInspectionDate;
        NextInspectionDue = nextInspectionDue;
        NextInspectionHours = nextInspectionHours;
        CertificateExpiry = certificateExpiry;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsCertificateExpiringSoon(int daysThreshold = 30) =>
        CertificateExpiry <= DateTime.UtcNow.AddDays(daysThreshold);
}
