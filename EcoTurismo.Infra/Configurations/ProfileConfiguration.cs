using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("profiles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .HasComment("Identificador único do perfil (mesmo ID do auth)");

        builder.Property(p => p.Nome)
            .HasColumnName("nome")
            .HasComment("Nome completo do usuário")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Email)
            .HasColumnName("email")
            .HasComment("Endereço de e-mail do usuário")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Role)
            .HasColumnName("role")
            .HasComment("Papel do usuário no sistema (admin, operador, publico)")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.MunicipioId)
            .HasColumnName("municipio_id")
            .HasComment("FK para o município vinculado");

        builder.Property(p => p.AtrativoId)
            .HasColumnName("atrativo_id")
            .HasComment("FK para o atrativo vinculado");

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasComment("Data de criação do registro");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .HasComment("Data da última atualização do registro");

        builder.Property(p => p.PasswordHash)
            .HasColumnName("password_hash")
            .HasComment("Hash da senha do usuário");

        // Indexes
        builder.HasIndex(p => p.Email)
            .IsUnique();

        // Relationships
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
