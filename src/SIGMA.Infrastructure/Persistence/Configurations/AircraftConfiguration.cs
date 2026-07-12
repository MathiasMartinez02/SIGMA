using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGMA.Domain.Entities;

namespace SIGMA.Infrastructure.Persistence.Configurations;

public class AircraftConfiguration : IEntityTypeConfiguration<Aircraft>
{
    public void Configure(EntityTypeBuilder<Aircraft> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.Registration).IsUnique();
        builder.Property(a => a.Registration).IsRequired().HasMaxLength(10);
        builder.Property(a => a.Model).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Manufacturer).IsRequired().HasMaxLength(100);
        builder.Property(a => a.SerialNumber).HasMaxLength(100);
        builder.Property(a => a.EngineModel).HasMaxLength(100);
        builder.Property(a => a.EngineSerialNumber).HasMaxLength(100);
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(a => a.Category).HasConversion<string>().HasMaxLength(30);
        builder.Property(a => a.TotalFlightHours).HasPrecision(10, 1);
        builder.Property(a => a.TotalLandings).HasPrecision(10, 0);
        builder.Property(a => a.NextInspectionHours).HasPrecision(10, 1);

        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.HasOne(a => a.Client)
            .WithMany(c => c.Aircraft)
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Documents)
            .WithOne(d => d.Aircraft)
            .HasForeignKey(d => d.AircraftId);

        builder.HasMany(a => a.FlightRecords)
            .WithOne(f => f.Aircraft)
            .HasForeignKey(f => f.AircraftId);

        builder.HasMany(a => a.Components)
            .WithOne(c => c.Aircraft)
            .HasForeignKey(c => c.AircraftId);
    }
}

public class FlightRecordConfiguration : IEntityTypeConfiguration<FlightRecord>
{
    public void Configure(EntityTypeBuilder<FlightRecord> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Duration).HasPrecision(8, 1);
        builder.Property(f => f.Pilot).HasMaxLength(200);
        builder.Property(f => f.Origin).HasMaxLength(100);
        builder.Property(f => f.Destination).HasMaxLength(100);
        builder.Property(f => f.Notes).HasMaxLength(500);
        builder.HasIndex(f => f.Date);
    }
}

public class AircraftComponentConfiguration : IEntityTypeConfiguration<AircraftComponent>
{
    public void Configure(EntityTypeBuilder<AircraftComponent> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.PartNumber).HasMaxLength(100);
        builder.Property(c => c.SerialNumber).HasMaxLength(100);
        builder.Property(c => c.Manufacturer).HasMaxLength(150);
        builder.Property(c => c.Status).HasMaxLength(30);
        builder.Property(c => c.InstallHours).HasPrecision(10, 1);
        builder.Property(c => c.LifeLimitHours).HasPrecision(10, 1);
        builder.Property(c => c.OverhaulDueHours).HasPrecision(10, 1);
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}

public class AircraftDocumentConfiguration : IEntityTypeConfiguration<AircraftDocument>
{
    public void Configure(EntityTypeBuilder<AircraftDocument> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
        builder.Property(d => d.Type).HasMaxLength(100);
        builder.Property(d => d.FileUrl).HasMaxLength(500);
    }
}
