using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGMA.Domain.Entities;

namespace SIGMA.Infrastructure.Persistence.Configurations;

// Mapeo de Appointment: relacion obligatoria con Client, opcional con Aircraft, indices por fecha y estado
public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.AircraftRegistrationHint).HasMaxLength(20);
        builder.Property(a => a.RequestedType).HasConversion<string>().HasMaxLength(50);
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(a => a.Notes).HasMaxLength(1000);

        builder.HasIndex(a => a.ScheduledDate);
        builder.HasIndex(a => a.Status);

        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.HasOne(a => a.Client)
            .WithMany()
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Aircraft)
            .WithMany()
            .HasForeignKey(a => a.AircraftId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
