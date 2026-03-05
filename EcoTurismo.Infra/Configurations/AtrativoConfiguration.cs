using EcoTurismo.Domain.Entities;
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
            .HasComment("Tipo do atrativo (balneario, parque, etc.)")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.Descricao)
            .HasColumnName("Descricao")
            .HasComment("Descrição detalhada do atrativo");

        builder.Property(a => a.Imagem)
            .HasColumnName("Imagem")
            .HasComment("URL da imagem do atrativo");

        builder.Property(a => a.Imagens)
            .HasColumnName("Imagens")
            .HasComment("Array JSON de múltiplas imagens em base64")
            .HasColumnType("text");

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
