using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class QuiosqueConfiguration : IEntityTypeConfiguration<Quiosque>
{
    public void Configure(EntityTypeBuilder<Quiosque> builder)
    {
        builder.ToTable("Quiosques");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Id)
            .HasColumnName("Id")
            .HasComment("Identificador único do quiosque");

        builder.Property(q => q.AtrativoId)
            .HasColumnName("AtrativoId")
            .HasComment("FK para o atrativo ao qual o quiosque pertence");

        builder.Property(q => q.Numero)
            .HasColumnName("Numero")
            .HasComment("Número identificador do quiosque");

        builder.Property(q => q.TemChurrasqueira)
            .HasColumnName("TemChurrasqueira")
            .HasComment("Indica se o quiosque possui churrasqueira");

        builder.Property(q => q.Status)
            .HasColumnName("Status")
            .HasComment("Status do quiosque (disponivel, ocupado, manutencao)")
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(q => q.PosicaoX)
            .HasColumnName("PosicaoX")
            .HasComment("Posição X do quiosque no mapa");

        builder.Property(q => q.PosicaoY)
            .HasColumnName("PosicaoY")
            .HasComment("Posição Y do quiosque no mapa");

        builder.Property(q => q.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasComment("Data de criação do registro");

        builder.Property(q => q.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasComment("Data da última atualização do registro");

        // Indexes
        builder.HasIndex(q => q.AtrativoId)
            .HasDatabaseName("IX_Quiosques_AtrativoId");

        // Relationships
        builder.HasOne(q => q.Atrativo)
            .WithMany(a => a.Quiosques)
            .HasForeignKey(q => q.AtrativoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
