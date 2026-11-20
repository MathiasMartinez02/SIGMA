using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Entities;

namespace SIGMA.API.WorkOrders.Services;

public static class EditableWorkOrder
{
    // OT existente y abierta, o el mensaje de error para devolver
    public static async Task<(WorkOrder? WorkOrder, string? Error)> FindAsync(
        IApplicationDbContext context,
        Guid workOrderId,
        CancellationToken cancellationToken,
        Func<IQueryable<WorkOrder>, IQueryable<WorkOrder>>? include = null)
    {
        var query = include is null ? context.WorkOrders : include(context.WorkOrders);
        var workOrder = await query.FirstOrDefaultAsync(w => w.Id == workOrderId && !w.IsDeleted, cancellationToken);

        if (workOrder is null) return (null, "Orden de trabajo no encontrada.");
        if (workOrder.IsClosed) return (null, $"La OT está {workOrder.Status}: no admite cambios.");
        return (workOrder, null);
    }
}
