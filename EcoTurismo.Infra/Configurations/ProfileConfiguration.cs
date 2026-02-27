using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("Profiles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("Id")
            .HasComment("Identificador único do perfil (mesmo ID do auth)");

        builder.Property(p => p.Nome)
            .HasColumnName("Nome")
            .HasComment("Nome completo do usuário")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Email)
            .HasColumnName("Email")
            .HasComment("Endereço de e-mail do usuário")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.RoleId)
            .HasColumnName("RoleId")
            .HasComment("FK para a role do usuário");

        builder.Property(p => p.MunicipioId)
            .HasColumnName("MunicipioId")
            .HasComment("FK para o município vinculado");

        builder.Property(p => p.AtrativoId)
            .HasColumnName("AtrativoId")
            .HasComment("FK para o atrativo vinculado");

        builder.Property(p => p.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasComment("Data de criação do registro");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasComment("Data da última atualização do registro");

        builder.Property(p => p.PasswordHash)
            .HasColumnName("PasswordHash")
            .HasComment("Hash da senha do usuário");

        // Indexes
        builder.HasIndex(p => p.Email)
            .HasDatabaseName("IX_Profiles_Email")
            .IsUnique();

        builder.HasIndex(p => p.RoleId)
            .HasDatabaseName("IX_Profiles_RoleId");

        // Relationships
        builder.HasOne(p => p.Role)
            .WithMany(r => r.Profiles)
            .HasForeignKey(p => p.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Municipio)
            .WithMany(m => m.Profiles)
            .HasForeignKey(p => p.MunicipioId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Atrativo)
            .WithMany()
            .HasForeignKey(p => p.AtrativoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
