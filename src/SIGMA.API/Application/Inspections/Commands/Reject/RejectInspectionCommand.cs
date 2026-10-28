using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Inspections.Commands.Reject;

public record RejectInspectionCommand(Guid InspectionId, string RejectionReason) : IRequest<Result>;
