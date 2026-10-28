using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Inspections.DTOs;

namespace SIGMA.API.Inspections.Commands.Create;

// Comando para programar una nueva inspección sobre una orden de trabajo existente
public record CreateInspectionCommand(Guid WorkOrderId, DateTime ScheduledDate, Guid? InspectorId = null)
    : IRequest<Result<InspectionDto>>;
