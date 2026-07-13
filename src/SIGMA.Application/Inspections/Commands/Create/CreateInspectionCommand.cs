using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Inspections.DTOs;

namespace SIGMA.Application.Inspections.Commands.Create;

// Comando para programar una nueva inspección sobre una orden de trabajo existente
public record CreateInspectionCommand(Guid WorkOrderId, DateTime ScheduledDate, Guid? InspectorId = null)
    : IRequest<Result<InspectionDto>>;
