using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGMA.Domain.Entities;

namespace SIGMA.Infrastructure.Persistence.Configurations;

public class TechnicalDocumentConfiguration : IEntityTypeConfiguration<TechnicalDocument>
{
    public void Configure(EntityTypeBuilder<TechnicalDocument> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Type).HasConversion<string>().HasMaxLength(30);
        builder.Property(d => d.Title).IsRequired().HasMaxLength(200);
        builder.Property(d => d.ReferenceCode).HasMaxLength(100);
        builder.Property(d => d.FileUrl).IsRequired().HasMaxLength(500);
        builder.Property(d => d.Description).HasMaxLength(1000);

        builder.HasIndex(d => d.Type);
        builder.HasIndex(d => d.ExpiryDate);

        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
