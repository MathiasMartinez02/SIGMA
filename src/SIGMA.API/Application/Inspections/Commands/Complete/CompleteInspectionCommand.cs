using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Inspections.Commands.Complete;

public record CompleteInspectionCommand(Guid InspectionId, string? Observations) : IRequest<Result>;
