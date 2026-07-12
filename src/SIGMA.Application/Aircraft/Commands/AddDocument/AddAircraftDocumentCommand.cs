using MediatR;
using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Aircraft.Commands.AddDocument;

public record AddAircraftDocumentCommand(
    Guid AircraftId,
    string Type,
    string Name,
    string FileUrl,
    DateTime? ExpiryDate
) : IRequest<Result<AircraftDocumentDto>>;
