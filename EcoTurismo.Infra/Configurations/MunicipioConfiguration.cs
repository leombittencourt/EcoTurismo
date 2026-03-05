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

        builder.Property(m => m.LogoId)
            .HasColumnName("LogoId")
            .HasComment("FK para imagem do logo geral do município");

        builder.Property(m => m.LogoTelaLoginId)
            .HasColumnName("LogoTelaLoginId")
            .HasComment("FK para imagem do logo da tela de login");

        builder.Property(m => m.LogoAreaPublicaId)
            .HasColumnName("LogoAreaPublicaId")
            .HasComment("FK para imagem do logo da área pública");

        builder.Property(m => m.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasComment("Data de criação do registro");

        // Indexes
        builder.HasIndex(m => m.LogoId)
            .HasDatabaseName("IX_Municipios_LogoId");

        builder.HasIndex(m => m.LogoTelaLoginId)
            .HasDatabaseName("IX_Municipios_LogoTelaLoginId");

        builder.HasIndex(m => m.LogoAreaPublicaId)
            .HasDatabaseName("IX_Municipios_LogoAreaPublicaId");

        // Relationships - Imagens
        builder.HasOne(m => m.Logo)
            .WithMany()
            .HasForeignKey(m => m.LogoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.LogoTelaLogin)
            .WithMany()
            .HasForeignKey(m => m.LogoTelaLoginId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.LogoAreaPublica)
            .WithMany()
            .HasForeignKey(m => m.LogoAreaPublicaId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relationships - Outras entidades
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
