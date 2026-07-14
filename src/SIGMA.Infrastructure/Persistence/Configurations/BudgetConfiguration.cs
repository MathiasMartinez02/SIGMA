using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGMA.Domain.Entities;

namespace SIGMA.Infrastructure.Persistence.Configurations;

// Mapeo de Budget: relacion obligatoria con Client, opcionales con Aircraft y Appointment, sin cascada
public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Description).HasMaxLength(2000).IsRequired();
        builder.Property(b => b.Amount).HasPrecision(12, 2);
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(b => b.Notes).HasMaxLength(1000);

        builder.HasIndex(b => b.Status);
        builder.HasIndex(b => b.ValidUntil);

        builder.HasQueryFilter(b => !b.IsDeleted);

        builder.HasOne(b => b.Client)
            .WithMany()
            .HasForeignKey(b => b.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Aircraft)
            .WithMany()
            .HasForeignKey(b => b.AircraftId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Appointment)
            .WithMany()
            .HasForeignKey(b => b.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
