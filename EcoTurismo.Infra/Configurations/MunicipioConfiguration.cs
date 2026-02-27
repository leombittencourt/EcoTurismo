using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class MunicipioConfiguration : IEntityTypeConfiguration<Municipio>
{
    public void Configure(EntityTypeBuilder<Municipio> builder)
    {
        builder.ToTable("municipios");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("id")
            .HasComment("Identificador único do município");

        builder.Property(m => m.Nome)
            .HasColumnName("nome")
            .HasComment("Nome do município")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Uf)
            .HasColumnName("uf")
            .HasComment("Unidade federativa (sigla do estado)")
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(m => m.Logo)
            .HasColumnName("logo")
            .HasComment("URL do logotipo do município");

        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at")
            .HasComment("Data de criação do registro");

        // Relationships
        builder.HasMany(m => m.Atrativos)
            .WithOne(a => a.Municipio)
            .HasForeignKey(a => a.MunicipioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Profiles)
            .WithOne(p => p.Municipio)
            .HasForeignKey(p => p.MunicipioId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
