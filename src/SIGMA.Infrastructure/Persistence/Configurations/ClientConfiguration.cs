using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGMA.Domain.Entities;

namespace SIGMA.Infrastructure.Persistence.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.TaxId).IsUnique();
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.BusinessName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.TaxId).IsRequired().HasMaxLength(20);
        builder.Property(c => c.Email).HasMaxLength(256);
        builder.Property(c => c.Phone).HasMaxLength(30);
        builder.Property(c => c.Address).HasMaxLength(300);
        builder.Property(c => c.City).HasMaxLength(100);
        builder.Property(c => c.Province).HasMaxLength(100);
        builder.Property(c => c.ContactPerson).HasMaxLength(150);
        builder.Property(c => c.ContactPhone).HasMaxLength(30);

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
