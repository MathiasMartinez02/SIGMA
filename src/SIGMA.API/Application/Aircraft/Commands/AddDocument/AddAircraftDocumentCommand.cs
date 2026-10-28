using MediatR;
using SIGMA.API.Aircraft.DTOs;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Aircraft.Commands.AddDocument;

public record AddAircraftDocumentCommand(
    Guid AircraftId,
    string Type,
    string Name,
    string FileUrl,
    DateTime? ExpiryDate
) : IRequest<Result<AircraftDocumentDto>>;
