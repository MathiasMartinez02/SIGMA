using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGMA.Domain.Entities;

namespace SIGMA.Infrastructure.Persistence.Configurations;

public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.HasKey(w => w.Id);
        builder.HasIndex(w => w.Number).IsUnique();
        builder.Property(w => w.Number).IsRequired().HasMaxLength(20);
        builder.Property(w => w.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(w => w.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(w => w.Priority).HasConversion<string>().HasMaxLength(20);
        builder.Property(w => w.Description).IsRequired().HasMaxLength(2000);
        builder.Property(w => w.EstimatedHours).HasPrecision(8, 2);
        builder.Property(w => w.ActualHours).HasPrecision(8, 2);
        builder.Property(w => w.AircraftHoursAtStart).HasPrecision(10, 1);
        // Mapea la fecha de ingreso de la aeronave al taller (dato de negocio, distinto de CreatedAt de auditoria)
        builder.Property(w => w.IntakeDate).IsRequired();

        builder.HasIndex(w => w.Status);
        builder.HasIndex(w => w.Priority);
        builder.HasIndex(w => w.CreatedAt);

        builder.HasQueryFilter(w => !w.IsDeleted);

        builder.HasOne(w => w.Aircraft)
            .WithMany(a => a.WorkOrders)
            .HasForeignKey(w => w.AircraftId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Client)
            .WithMany(c => c.WorkOrders)
            .HasForeignKey(w => w.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(w => w.Tasks)
            .WithOne(t => t.WorkOrder)
            .HasForeignKey(t => t.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.Timeline)
            .WithOne(t => t.WorkOrder)
            .HasForeignKey(t => t.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.Materials)
            .WithOne(m => m.WorkOrder)
            .HasForeignKey(m => m.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.Documents)
            .WithOne(d => d.WorkOrder)
            .HasForeignKey(d => d.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.AssignedMechanics)
            .WithOne(am => am.WorkOrder)
            .HasForeignKey(am => am.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class WorkOrderTaskConfiguration : IEntityTypeConfiguration<WorkOrderTask>
{
    public void Configure(EntityTypeBuilder<WorkOrderTask> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).IsRequired().HasMaxLength(300);
        builder.Property(t => t.Description).HasMaxLength(2000);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(t => t.EstimatedHours).HasPrecision(8, 2);
        builder.Property(t => t.ActualHours).HasPrecision(8, 2);
        builder.Property(t => t.Observations).HasMaxLength(1000);

        builder.HasOne(t => t.AssignedTo)
            .WithMany()
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(t => t.InspectedBy)
            .WithMany()
            .HasForeignKey(t => t.InspectedById)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class WorkOrderMaterialConfiguration : IEntityTypeConfiguration<WorkOrderMaterial>
{
    public void Configure(EntityTypeBuilder<WorkOrderMaterial> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.PartNumber).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Description).IsRequired().HasMaxLength(500);
        builder.Property(m => m.Unit).HasMaxLength(30);
        builder.Property(m => m.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(m => m.Quantity).HasPrecision(12, 3);
    }
}

public class WorkOrderTimelineConfiguration : IEntityTypeConfiguration<WorkOrderTimeline>
{
    public void Configure(EntityTypeBuilder<WorkOrderTimeline> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Event).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.UserName).HasMaxLength(200);
        builder.Property(t => t.UserRole).HasMaxLength(50);
        builder.Property(t => t.Metadata).HasColumnType("nvarchar(max)");
        builder.HasIndex(t => t.CreatedAt);
    }
}

public class AssignedMechanicConfiguration : IEntityTypeConfiguration<AssignedMechanic>
{
    public void Configure(EntityTypeBuilder<AssignedMechanic> builder)
    {
        builder.HasKey(am => new { am.WorkOrderId, am.UserId });
        builder.Property(am => am.Role).HasMaxLength(50);

        builder.HasOne(am => am.User)
            .WithMany()
            .HasForeignKey(am => am.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
