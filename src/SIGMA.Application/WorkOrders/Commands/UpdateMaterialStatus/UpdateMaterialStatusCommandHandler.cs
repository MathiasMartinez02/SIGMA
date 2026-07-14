using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.WorkOrders.Commands.UpdateMaterialStatus;

// Aplica la transicion de estado de un material de OT usando los metodos de dominio ya existentes (MarkDelivered/MarkUnavailable)
public class UpdateMaterialStatusCommandHandler : IRequestHandler<UpdateMaterialStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateMaterialStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateMaterialStatusCommand request, CancellationToken cancellationToken)
    {
        var material = await _context.WorkOrderMaterials
            .FirstOrDefaultAsync(m => m.Id == request.MaterialId && m.WorkOrderId == request.WorkOrderId, cancellationToken);

        if (material is null)
            return Result.Failure("Material no encontrado.");

        switch (request.Status)
        {
            case WorkOrderMaterialStatus.Entregado:
                material.MarkDelivered();
                break;
            case WorkOrderMaterialStatus.NoDisponible:
                material.MarkUnavailable();
                break;
            default:
                return Result.Failure("Solo se puede marcar un material como Entregado o No Disponible.");
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
