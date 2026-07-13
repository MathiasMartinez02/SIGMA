using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGMA.Domain.Entities;

namespace SIGMA.Infrastructure.Persistence.Configurations;

// Configuracion EF Core de Notification, con indices por UserId e IsRead para las consultas del usuario
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Type).HasConversion<string>().HasMaxLength(30);
        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
        builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);

        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.IsRead);

        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
