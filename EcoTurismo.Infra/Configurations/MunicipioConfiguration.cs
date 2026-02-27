using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class MunicipioConfiguration : IEntityTypeConfiguration<Municipio>
{
    public void Configure(EntityTypeBuilder<Municipio> builder)
    {
        builder.ToTable("Municipios");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("Id")
            .HasComment("Identificador único do município");

        builder.Property(m => m.Nome)
            .HasColumnName("Nome")
            .HasComment("Nome do município")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Uf)
            .HasColumnName("Uf")
            .HasComment("Unidade federativa (sigla do estado)")
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(m => m.Logo)
            .HasColumnName("Logo")
            .HasComment("URL do logotipo do município");

        builder.Property(m => m.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasComment("Data de criação do registro");

        // Relationships
        builder.HasMany(m => m.Atrativos)
            .WithOne(a => a.Municipio)
            .HasForeignKey(a => a.MunicipioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Usuarios)
            .WithOne(u => u.Municipio)
            .HasForeignKey(u => u.MunicipioId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
