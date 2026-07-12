using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGMA.Domain.Entities;

namespace SIGMA.Infrastructure.Persistence.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.HasIndex(i => i.PartNumber).IsUnique();
        builder.Property(i => i.PartNumber).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Description).IsRequired().HasMaxLength(500);
        builder.Property(i => i.Category).HasConversion<string>().HasMaxLength(50);
        builder.Property(i => i.Manufacturer).HasMaxLength(150);
        builder.Property(i => i.Location).HasMaxLength(100);
        builder.Property(i => i.Unit).HasMaxLength(30);
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(i => i.AltPartNumbers).HasColumnType("nvarchar(max)");
        builder.Property(i => i.CertificateNumber).HasMaxLength(100);
        builder.Property(i => i.CurrentStock).HasPrecision(12, 3);
        builder.Property(i => i.MinimumStock).HasPrecision(12, 3);
        builder.Property(i => i.UnitCost).HasPrecision(12, 2);

        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.Category);

        builder.HasQueryFilter(i => !i.IsDeleted);

        builder.HasMany(i => i.Movements)
            .WithOne(m => m.Item)
            .HasForeignKey(m => m.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Type).HasConversion<string>().HasMaxLength(30);
        builder.Property(m => m.Quantity).HasPrecision(12, 3);
        builder.Property(m => m.PreviousStock).HasPrecision(12, 3);
        builder.Property(m => m.NewStock).HasPrecision(12, 3);
        builder.Property(m => m.Reason).HasMaxLength(500);

        builder.HasIndex(m => m.PerformedAt);

        builder.HasOne(m => m.PerformedBy)
            .WithMany()
            .HasForeignKey(m => m.PerformedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
