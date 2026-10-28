using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.Inspections.Commands.UpdateChecklistItem;

public record UpdateChecklistItemCommand(
    Guid InspectionId,
    Guid ItemId,
    ChecklistItemStatus Status,
    string? Observations
) : IRequest<Result>;
