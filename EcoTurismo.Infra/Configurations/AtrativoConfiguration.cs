using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class AtrativoConfiguration : IEntityTypeConfiguration<Atrativo>
{
    public void Configure(EntityTypeBuilder<Atrativo> builder)
    {
        builder.ToTable("atrativos");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id")
            .HasComment("Identificador único do atrativo");

        builder.Property(a => a.MunicipioId)
            .HasColumnName("municipio_id")
            .HasComment("FK para o município ao qual o atrativo pertence");

        builder.Property(a => a.Nome)
            .HasColumnName("nome")
            .HasComment("Nome do atrativo turístico")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Tipo)
            .HasColumnName("tipo")
            .HasComment("Tipo do atrativo (balneario, parque, etc.)")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.Descricao)
            .HasColumnName("descricao")
            .HasComment("Descrição detalhada do atrativo");

        builder.Property(a => a.Imagem)
            .HasColumnName("imagem")
            .HasComment("URL da imagem do atrativo");

        builder.Property(a => a.CapacidadeMaxima)
            .HasColumnName("capacidade_maxima")
            .HasComment("Capacidade máxima de visitantes");

        builder.Property(a => a.OcupacaoAtual)
            .HasColumnName("ocupacao_atual")
            .HasComment("Quantidade atual de visitantes no local");

        builder.Property(a => a.Status)
            .HasColumnName("status")
            .HasComment("Status do atrativo (ativo, inativo)")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .HasComment("Data de criação do registro");

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .HasComment("Data da última atualização do registro");

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
