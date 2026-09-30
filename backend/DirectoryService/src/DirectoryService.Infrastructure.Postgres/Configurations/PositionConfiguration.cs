using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasConversion(p => p.Value, v => Name.Create(v))
            .HasMaxLength(Name.MaxLength);

        builder.Property(p => p.CreatedAt)
            .HasColumnType("timestamptz");
        
        builder.Property(p => p.UpdatedAt)
            .HasColumnType("timestamptz");
        
        builder.HasIndex(p => p.Name)
            .IsUnique();
    }
}