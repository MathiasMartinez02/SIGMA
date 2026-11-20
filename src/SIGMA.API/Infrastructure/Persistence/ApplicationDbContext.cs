using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Entities;
using AircraftEntity = SIGMA.API.Entities.Aircraft;

namespace SIGMA.API.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<AircraftEntity> Aircraft => Set<AircraftEntity>();
    public DbSet<AircraftDocument> AircraftDocuments => Set<AircraftDocument>();
    public DbSet<FlightRecord> FlightRecords => Set<FlightRecord>();
    public DbSet<AircraftComponent> AircraftComponents => Set<AircraftComponent>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<WorkOrderTask> WorkOrderTasks => Set<WorkOrderTask>();
    public DbSet<WorkOrderTimeline> WorkOrderTimelines => Set<WorkOrderTimeline>();
    public DbSet<WorkOrderMaterial> WorkOrderMaterials => Set<WorkOrderMaterial>();
    public DbSet<WorkOrderDocument> WorkOrderDocuments => Set<WorkOrderDocument>();
    public DbSet<AssignedMechanic> AssignedMechanics => Set<AssignedMechanic>();
    public DbSet<Inspection> Inspections => Set<Inspection>();
    public DbSet<ChecklistSection> ChecklistSections => Set<ChecklistSection>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    // Fase 4: repositorio general de documentacion tecnica (manuales, boletines, directivas AD, certificados)
    public DbSet<TechnicalDocument> TechnicalDocuments => Set<TechnicalDocument>();
    // Fase 6: presupuestos (MVP de Administrativo)
    public DbSet<Budget> Budgets => Set<Budget>();

    // Configuraciones EF y claves generadas por el dominio
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(t => typeof(SIGMA.API.Common.BaseEntity).IsAssignableFrom(t.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .Property(nameof(SIGMA.API.Common.BaseEntity.Id))
                .ValueGeneratedNever();
        }

        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => IsUtcTimestamp(p.ClrType, p.Name)))
        {
            property.SetValueConverter(property.ClrType == typeof(DateTime) ? UtcConverter : NullableUtcConverter);
        }
    }

    private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
        new(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    private static readonly ValueConverter<DateTime?, DateTime?> NullableUtcConverter =
        new(v => v, v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

    private static readonly HashSet<string> UtcTimestampNames = ["StartDate", "CompletedDate", "LastMovementDate"];

    // Instantes guardados en UTC (terminan en At o son marcas de inicio/fin); el resto son fechas de negocio
    private static bool IsUtcTimestamp(Type type, string name) =>
        (type == typeof(DateTime) || type == typeof(DateTime?))
        && (name.EndsWith("At", StringComparison.Ordinal) || UtcTimestampNames.Contains(name));

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<SIGMA.API.Common.AuditableEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
