using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Inspections.Commands.Approve;

public record ApproveInspectionCommand(Guid InspectionId, string? Observations) : IRequest<Result>;
