using EcoTurismo.Domain.Entities;
using EcoTurismo.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class AtrativoConfiguration : IEntityTypeConfiguration<Atrativo>
{
    public void Configure(EntityTypeBuilder<Atrativo> builder)
    {
        builder.ToTable("Atrativos");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("Id")
            .HasComment("Identificador único do atrativo");

        builder.Property(a => a.MunicipioId)
            .HasColumnName("MunicipioId")
            .HasComment("FK para o município ao qual o atrativo pertence");

        builder.Property(a => a.Nome)
            .HasColumnName("Nome")
            .HasComment("Nome do atrativo turístico")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Tipo)
            .HasColumnName("Tipo")
            .HasComment("Tipo do atrativo (balneario, cachoeira, trilha, parque, fazenda_ecoturismo)")
            .IsRequired()
            .HasMaxLength(25)
            .HasConversion(
                v => v.ToStringValue(),
                v => TipoAtrativoExtensions.FromString(v)
            );

        builder.Property(a => a.Descricao)
            .HasColumnName("Descricao")
            .HasComment("Descrição detalhada do atrativo");

        builder.Property(a => a.Endereco)
            .HasColumnName("Endereco")
            .HasComment("Endereço do atrativo")
            .HasMaxLength(500);

        builder.Property(a => a.Latitude)
            .HasColumnName("Latitude")
            .HasComment("Latitude do atrativo")
            .HasPrecision(10, 7);

        builder.Property(a => a.Longitude)
            .HasColumnName("Longitude")
            .HasComment("Longitude do atrativo")
            .HasPrecision(10, 7);

        builder.Property(a => a.MapUrl)
            .HasColumnName("MapUrl")
            .HasComment("URL do mapa (Google Maps, etc)")
            .HasMaxLength(1000);

        builder.Property(a => a.CapacidadeMaxima)
            .HasColumnName("CapacidadeMaxima")
            .HasComment("Capacidade máxima de visitantes");

        builder.Property(a => a.OcupacaoAtual)
            .HasColumnName("OcupacaoAtual")
            .HasComment("Quantidade atual de visitantes no local");

        builder.Property(a => a.Status)
            .HasColumnName("Status")
            .HasComment("Status do atrativo (ativo, inativo)")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasComment("Data de criação do registro");

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasComment("Data da última atualização do registro");

        // Indexes
        builder.HasIndex(a => a.MunicipioId)
            .HasDatabaseName("IX_Atrativos_MunicipioId");

        // Relationships
        builder.HasOne(a => a.Municipio)
            .WithMany(m => m.Atrativos)
            .HasForeignKey(a => a.MunicipioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Reservas)
            .WithOne(r => r.Atrativo)
            .HasForeignKey(r => r.AtrativoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Quiosques)
            .WithOne(q => q.Atrativo)
            .HasForeignKey(q => q.AtrativoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
