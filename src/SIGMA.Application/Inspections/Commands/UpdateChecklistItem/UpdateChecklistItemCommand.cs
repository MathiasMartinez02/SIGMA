using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Inspections.Commands.UpdateChecklistItem;

public record UpdateChecklistItemCommand(
    Guid InspectionId,
    Guid ItemId,
    ChecklistItemStatus Status,
    string? Observations
) : IRequest<Result>;
