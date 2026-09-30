using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");
        
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name)
            .HasConversion(l => l.Value, v => Name.Create(v))
            .HasMaxLength(Name.MaxLength);

        builder.Property(l => l.Address)
            .HasConversion(l => l.Value, v => Address.Create(v))
            .HasMaxLength(Address.MaxLength);

        builder.Property(l => l.CreatedAt)
            .HasColumnType("timestamptz");
        
        builder.Property(l => l.UpdatedAt)
            .HasColumnType("timestamptz");
        
        builder.HasIndex(l => l.Address)
            .IsUnique();
        
        builder.HasIndex(l => l.Name)
            .IsUnique();
    }
}