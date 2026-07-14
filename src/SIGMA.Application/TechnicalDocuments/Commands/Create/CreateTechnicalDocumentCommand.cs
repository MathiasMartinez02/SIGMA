using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.TechnicalDocuments.DTOs;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.TechnicalDocuments.Commands.Create;

public record CreateTechnicalDocumentCommand(
    TechnicalDocumentType Type,
    string Title,
    string FileUrl,
    string? ReferenceCode = null,
    string? Description = null,
    DateTime? IssueDate = null,
    DateTime? ExpiryDate = null
) : IRequest<Result<TechnicalDocumentDto>>;
