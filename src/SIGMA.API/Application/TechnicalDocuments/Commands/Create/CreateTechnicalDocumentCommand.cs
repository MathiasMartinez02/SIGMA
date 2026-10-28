using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.TechnicalDocuments.DTOs;
using SIGMA.API.Enums;

namespace SIGMA.API.TechnicalDocuments.Commands.Create;

public record CreateTechnicalDocumentCommand(
    TechnicalDocumentType Type,
    string Title,
    string FileUrl,
    string? ReferenceCode = null,
    string? Description = null,
    DateTime? IssueDate = null,
    DateTime? ExpiryDate = null
) : IRequest<Result<TechnicalDocumentDto>>;
