using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Entities;
using SIGMA.API.Enums;
using AircraftEntity = SIGMA.API.Entities.Aircraft;

namespace SIGMA.API.WorkOrders.Services;

public static class AircraftStatusSync
{
    // Estado de la aeronave segun todas sus OT activas, incluida la que se esta modificando
    public static async Task ApplyAsync(IApplicationDbContext context, WorkOrder current,
        AircraftEntity aircraft, CancellationToken cancellationToken)
    {
        var statuses = await context.WorkOrders
            .Where(w => w.AircraftId == current.AircraftId && w.Id != current.Id && !w.IsDeleted
                        && (w.Status == WorkOrderStatus.EnProceso || w.Status == WorkOrderStatus.EnInspeccion))
            .Select(w => w.Status)
            .ToListAsync(cancellationToken);

        statuses.Add(current.Status);

        aircraft.SyncMaintenanceStatus(
            statuses.Contains(WorkOrderStatus.EnInspeccion),
            statuses.Contains(WorkOrderStatus.EnProceso));
    }
}
