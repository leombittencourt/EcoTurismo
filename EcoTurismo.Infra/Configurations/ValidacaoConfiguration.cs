using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class ValidacaoConfiguration : IEntityTypeConfiguration<Validacao>
{
    public void Configure(EntityTypeBuilder<Validacao> builder)
    {
        builder.ToTable("Validacoes");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasColumnName("Id")
            .HasComment("Identificador único da validação");

        builder.Property(v => v.ReservaId)
            .HasColumnName("ReservaId")
            .HasComment("FK para a reserva validada");

        builder.Property(v => v.AtrativoId)
            .HasColumnName("AtrativoId")
            .HasComment("FK para o atrativo onde ocorreu a validação");

        builder.Property(v => v.OperadorId)
            .HasColumnName("OperadorId")
            .HasComment("FK para o operador que realizou a validação");

        builder.Property(v => v.Token)
            .HasColumnName("Token")
            .HasComment("Token validado")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Valido)
            .HasColumnName("Valido")
            .HasComment("Indica se o token era válido no momento da validação");

        builder.Property(v => v.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasComment("Data de criação do registro");

        // Indexes
        builder.HasIndex(v => v.Token)
            .HasDatabaseName("IX_Validacoes_Token");

        // Relationships
        builder.HasOne(v => v.Reserva)
            .WithMany()
            .HasForeignKey(v => v.ReservaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(v => v.Atrativo)
            .WithMany()
            .HasForeignKey(v => v.AtrativoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(v => v.Operador)
            .WithMany()
            .HasForeignKey(v => v.OperadorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
