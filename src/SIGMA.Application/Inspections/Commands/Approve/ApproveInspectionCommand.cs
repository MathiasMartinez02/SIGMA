using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Inspections.Commands.Approve;

public record ApproveInspectionCommand(Guid InspectionId, string? Observations) : IRequest<Result>;
