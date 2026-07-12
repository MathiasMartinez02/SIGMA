using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGMA.Domain.Entities;

namespace SIGMA.Infrastructure.Persistence.Configurations;

public class InspectionConfiguration : IEntityTypeConfiguration<Inspection>
{
    public void Configure(EntityTypeBuilder<Inspection> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Type).HasMaxLength(100);
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(i => i.AircraftHours).HasPrecision(10, 1);
        builder.Property(i => i.RejectionReason).HasMaxLength(1000);
        builder.Property(i => i.OverallResult).HasMaxLength(100);
        builder.Property(i => i.Observations).HasMaxLength(2000);

        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.ScheduledDate);

        builder.HasOne(i => i.WorkOrder)
            .WithMany(w => w.Inspections)
            .HasForeignKey(i => i.WorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Aircraft)
            .WithMany()
            .HasForeignKey(i => i.AircraftId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Inspector)
            .WithMany()
            .HasForeignKey(i => i.InspectorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(i => i.ApprovedBy)
            .WithMany()
            .HasForeignKey(i => i.ApprovedById)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(i => i.ChecklistSections)
            .WithOne(s => s.Inspection)
            .HasForeignKey(s => s.InspectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ChecklistSectionConfiguration : IEntityTypeConfiguration<ChecklistSection>
{
    public void Configure(EntityTypeBuilder<ChecklistSection> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Title).IsRequired().HasMaxLength(200);

        builder.HasMany(s => s.Items)
            .WithOne(i => i.Section)
            .HasForeignKey(i => i.SectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ChecklistItemConfiguration : IEntityTypeConfiguration<ChecklistItem>
{
    public void Configure(EntityTypeBuilder<ChecklistItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Code).IsRequired().HasMaxLength(50);
        builder.Property(i => i.Description).IsRequired().HasMaxLength(500);
        builder.Property(i => i.Reference).HasMaxLength(200);
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(i => i.Observations).HasMaxLength(1000);
        builder.Property(i => i.PhotoUrl).HasMaxLength(500);
    }
}
