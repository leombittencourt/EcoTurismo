using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class ValidacaoConfiguration : IEntityTypeConfiguration<Validacao>
{
    public void Configure(EntityTypeBuilder<Validacao> builder)
    {
        builder.ToTable("validacoes");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasColumnName("id")
            .HasComment("Identificador único da validação");

        builder.Property(v => v.ReservaId)
            .HasColumnName("reserva_id")
            .HasComment("FK para a reserva validada");

        builder.Property(v => v.AtrativoId)
            .HasColumnName("atrativo_id")
            .HasComment("FK para o atrativo onde ocorreu a validação");

        builder.Property(v => v.OperadorId)
            .HasColumnName("operador_id")
            .HasComment("FK para o operador que realizou a validação");

        builder.Property(v => v.Token)
            .HasColumnName("token")
            .HasComment("Token validado")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Valido)
            .HasColumnName("valido")
            .HasComment("Indica se o token era válido no momento da validação");

        builder.Property(v => v.CreatedAt)
            .HasColumnName("created_at")
            .HasComment("Data de criação do registro");

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
