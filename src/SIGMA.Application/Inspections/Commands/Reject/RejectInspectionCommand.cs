using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Inspections.Commands.Reject;

public record RejectInspectionCommand(Guid InspectionId, string RejectionReason) : IRequest<Result>;
