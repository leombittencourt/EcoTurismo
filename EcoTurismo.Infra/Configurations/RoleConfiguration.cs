using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("Id")
            .HasComment("Identificador único da role");

        builder.Property(r => r.Name)
            .HasColumnName("Name")
            .HasComment("Nome da role (ex: Admin, Prefeitura)")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.NormalizedName)
            .HasColumnName("NormalizedName")
            .HasComment("Nome normalizado em maiúsculas para busca")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasColumnName("Description")
            .HasComment("Descrição da finalidade da role")
            .HasMaxLength(500);

        builder.Property(r => r.IsActive)
            .HasColumnName("IsActive")
            .HasComment("Indica se a role está ativa")
            .HasDefaultValue(true);

        builder.Property(r => r.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasComment("Data de criação do registro");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasComment("Data da última atualização do registro");

        // Indexes
        builder.HasIndex(r => r.NormalizedName)
            .HasDatabaseName("IX_Roles_NormalizedName")
            .IsUnique();

        // Relationships
        builder.HasMany(r => r.Usuarios)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.RolePermissions)
            .WithOne(rp => rp.Role)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
