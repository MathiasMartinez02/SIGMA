using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGMA.Domain.Entities;

namespace SIGMA.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);
        builder.HasIndex(rt => rt.Token).IsUnique();
        builder.Property(rt => rt.Token).IsRequired().HasMaxLength(200);
    }
}
