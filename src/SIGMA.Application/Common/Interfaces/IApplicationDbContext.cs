using Microsoft.EntityFrameworkCore;

namespace SIGMA.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Domain.Entities.User> Users { get; }
    DbSet<Domain.Entities.RefreshToken> RefreshTokens { get; }
    DbSet<Domain.Entities.Client> Clients { get; }
    DbSet<Domain.Entities.Aircraft> Aircraft { get; }
    DbSet<Domain.Entities.AircraftDocument> AircraftDocuments { get; }
    DbSet<Domain.Entities.FlightRecord> FlightRecords { get; }
    DbSet<Domain.Entities.AircraftComponent> AircraftComponents { get; }
    DbSet<Domain.Entities.WorkOrder> WorkOrders { get; }
    DbSet<Domain.Entities.WorkOrderTask> WorkOrderTasks { get; }
    DbSet<Domain.Entities.WorkOrderTimeline> WorkOrderTimelines { get; }
    DbSet<Domain.Entities.WorkOrderMaterial> WorkOrderMaterials { get; }
    DbSet<Domain.Entities.WorkOrderDocument> WorkOrderDocuments { get; }
    DbSet<Domain.Entities.AssignedMechanic> AssignedMechanics { get; }
    DbSet<Domain.Entities.Inspection> Inspections { get; }
    DbSet<Domain.Entities.ChecklistSection> ChecklistSections { get; }
    DbSet<Domain.Entities.ChecklistItem> ChecklistItems { get; }
    DbSet<Domain.Entities.InventoryItem> InventoryItems { get; }
    DbSet<Domain.Entities.InventoryMovement> InventoryMovements { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
