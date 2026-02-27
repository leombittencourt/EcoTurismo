using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class QuiosqueConfiguration : IEntityTypeConfiguration<Quiosque>
{
    public void Configure(EntityTypeBuilder<Quiosque> builder)
    {
        builder.ToTable("quiosques");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Id)
            .HasColumnName("id")
            .HasComment("Identificador único do quiosque");

        builder.Property(q => q.AtrativoId)
            .HasColumnName("atrativo_id")
            .HasComment("FK para o atrativo ao qual o quiosque pertence");

        builder.Property(q => q.Numero)
            .HasColumnName("numero")
            .HasComment("Número identificador do quiosque");

        builder.Property(q => q.TemChurrasqueira)
            .HasColumnName("tem_churrasqueira")
            .HasComment("Indica se o quiosque possui churrasqueira");

        builder.Property(q => q.Status)
            .HasColumnName("status")
            .HasComment("Status do quiosque (disponivel, ocupado, manutencao)")
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(q => q.PosicaoX)
            .HasColumnName("posicao_x")
            .HasComment("Posição X do quiosque no mapa");

        builder.Property(q => q.PosicaoY)
            .HasColumnName("posicao_y")
            .HasComment("Posição Y do quiosque no mapa");

        builder.Property(q => q.CreatedAt)
            .HasColumnName("created_at")
            .HasComment("Data de criação do registro");

        builder.Property(q => q.UpdatedAt)
            .HasColumnName("updated_at")
            .HasComment("Data da última atualização do registro");

        // Indexes
        builder.HasIndex(q => q.AtrativoId);

        // Relationships
        builder.HasOne(q => q.Atrativo)
            .WithMany(a => a.Quiosques)
            .HasForeignKey(q => q.AtrativoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
