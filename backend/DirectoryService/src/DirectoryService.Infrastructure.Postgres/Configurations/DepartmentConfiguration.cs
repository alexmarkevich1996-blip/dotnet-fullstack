using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Path = DirectoryService.Domain.Departments.ValueObjects.Path;     

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");
        
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .HasConversion(n => n.Value, v => Name.Create(v))
            .HasMaxLength(Name.MaxLength);
        
        builder.Property(d => d.Slug)
            .HasConversion(s => s.Value, v => Slug.Create(v))
            .HasMaxLength(Slug.MaxLength);

        builder.Property(d => d.Path)
            .HasConversion(p => p.Value, v => Path.FromValue(v))
            .HasMaxLength(Path.MaxLength);

        builder.Property(d => d.CreatedAt)
            .HasColumnType("timestamptz");
        
        builder.Property(d => d.UpdatedAt)
            .HasColumnType("timestamptz");
        
        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(d => d.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(d => new {d.ParentId, d.Slug})
            .IsUnique();
        
        builder.HasIndex(d => d.Path)
            .IsUnique();
    }
}