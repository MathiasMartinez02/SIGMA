using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Inspections.DTOs;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.Inspections.Commands.Create;

// Crea una inspección derivando aeronave, tipo y horas de vuelo desde la orden de trabajo asociada
public class CreateInspectionCommandHandler : IRequestHandler<CreateInspectionCommand, Result<InspectionDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateInspectionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<InspectionDto>> Handle(CreateInspectionCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _context.WorkOrders
            .Include(w => w.Aircraft)
            .FirstOrDefaultAsync(w => w.Id == request.WorkOrderId && !w.IsDeleted, cancellationToken);

        if (workOrder is null)
            return Result<InspectionDto>.Failure("Orden de trabajo no encontrada.");

        var inspection = Inspection.Create(
            workOrder.Id,
            workOrder.Type.ToString(),
            workOrder.AircraftId,
            workOrder.Aircraft.TotalFlightHours,
            request.ScheduledDate,
            request.InspectorId);

        _context.Inspections.Add(inspection);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new InspectionDto
        {
            Id = inspection.Id,
            WorkOrderId = inspection.WorkOrderId,
            WorkOrderNumber = workOrder.Number,
            Type = inspection.Type,
            Status = inspection.Status,
            AircraftId = inspection.AircraftId,
            AircraftRegistration = workOrder.Aircraft.Registration,
            AircraftHours = inspection.AircraftHours,
            InspectorId = inspection.InspectorId,
            InspectorName = null,
            ScheduledDate = inspection.ScheduledDate,
            StartedAt = inspection.StartedAt,
            CompletedAt = inspection.CompletedAt,
            ApprovedAt = inspection.ApprovedAt,
            OverallResult = inspection.OverallResult,
            Observations = inspection.Observations,
            RejectionReason = inspection.RejectionReason,
            CreatedAt = inspection.CreatedAt
        };

        return Result<InspectionDto>.Success(dto);
    }
}
