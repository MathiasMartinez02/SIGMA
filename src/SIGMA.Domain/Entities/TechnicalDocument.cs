using SIGMA.Domain.Common;
using SIGMA.Domain.Enums;

namespace SIGMA.Domain.Entities;

// Documento tecnico general del taller (manual de mantenimiento, boletin de servicio, directiva AD, certificado),
// separado de AircraftDocument porque no esta atado a una matricula puntual sino que aplica a la flota/al taller en general.
public class TechnicalDocument : AuditableEntity
{
    public TechnicalDocumentType Type { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? ReferenceCode { get; private set; }
    public string FileUrl { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime? IssueDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;

    private TechnicalDocument() { }

    // Crea un documento tecnico del repositorio general (manual, boletin, directiva AD o certificado)
    public static TechnicalDocument Create(
        TechnicalDocumentType type,
        string title,
        string fileUrl,
        string? referenceCode = null,
        string? description = null,
        DateTime? issueDate = null,
        DateTime? expiryDate = null) =>
        new()
        {
            Type = type,
            Title = title,
            FileUrl = fileUrl,
            ReferenceCode = referenceCode,
            Description = description,
            IssueDate = issueDate,
            ExpiryDate = expiryDate
        };
}
