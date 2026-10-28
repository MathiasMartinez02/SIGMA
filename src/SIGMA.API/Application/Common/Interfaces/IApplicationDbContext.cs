using Microsoft.EntityFrameworkCore;
using SIGMA.API.Entities;
using AircraftEntity = SIGMA.API.Entities.Aircraft;

namespace SIGMA.API.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Client> Clients { get; }
    DbSet<AircraftEntity> Aircraft { get; }
    DbSet<AircraftDocument> AircraftDocuments { get; }
    DbSet<FlightRecord> FlightRecords { get; }
    DbSet<AircraftComponent> AircraftComponents { get; }
    DbSet<WorkOrder> WorkOrders { get; }
    DbSet<WorkOrderTask> WorkOrderTasks { get; }
    DbSet<WorkOrderTimeline> WorkOrderTimelines { get; }
    DbSet<WorkOrderMaterial> WorkOrderMaterials { get; }
    DbSet<WorkOrderDocument> WorkOrderDocuments { get; }
    DbSet<AssignedMechanic> AssignedMechanics { get; }
    DbSet<SIGMA.API.Entities.Inspection> Inspections { get; }
    DbSet<ChecklistSection> ChecklistSections { get; }
    DbSet<ChecklistItem> ChecklistItems { get; }
    DbSet<InventoryItem> InventoryItems { get; }
    DbSet<InventoryMovement> InventoryMovements { get; }
    DbSet<SIGMA.API.Entities.Notification> Notifications { get; }
    DbSet<SIGMA.API.Entities.Appointment> Appointments { get; }
    DbSet<TechnicalDocument> TechnicalDocuments { get; }
    DbSet<Budget> Budgets { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
