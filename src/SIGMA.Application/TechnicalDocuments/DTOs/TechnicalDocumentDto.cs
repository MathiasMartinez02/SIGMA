using SIGMA.Domain.Enums;

namespace SIGMA.Application.TechnicalDocuments.DTOs;

public class TechnicalDocumentDto
{
    public Guid Id { get; init; }
    public TechnicalDocumentType Type { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? ReferenceCode { get; init; }
    public string FileUrl { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime? IssueDate { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public bool IsExpired { get; init; }
    public DateTime CreatedAt { get; init; }
}
