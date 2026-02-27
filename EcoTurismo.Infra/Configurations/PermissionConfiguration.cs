using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("Id")
            .HasComment("Identificador único da permissão");

        builder.Property(p => p.Name)
            .HasColumnName("Name")
            .HasComment("Nome completo da permissão (ex: banners:create)")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Resource)
            .HasColumnName("Resource")
            .HasComment("Recurso da permissão (ex: banners, atrativos)")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Action)
            .HasColumnName("Action")
            .HasComment("Ação da permissão (ex: create, read, update, delete)")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Description)
            .HasColumnName("Description")
            .HasComment("Descrição da finalidade da permissão")
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasComment("Data de criação do registro");

        // Indexes
        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_Permissions_Name")
            .IsUnique();

        builder.HasIndex(p => new { p.Resource, p.Action })
            .HasDatabaseName("IX_Permissions_Resource_Action")
            .IsUnique();

        // Relationships
        builder.HasMany(p => p.RolePermissions)
            .WithOne(rp => rp.Permission)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
