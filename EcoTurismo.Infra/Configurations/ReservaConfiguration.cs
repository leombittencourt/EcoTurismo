using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
{
    public void Configure(EntityTypeBuilder<Reserva> builder)
    {
        builder.ToTable("Reservas");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("Id")
            .HasComment("Identificador único da reserva");

        builder.Property(r => r.AtrativoId)
            .HasColumnName("AtrativoId")
            .HasComment("FK para o atrativo reservado");

        builder.Property(r => r.QuiosqueId)
            .HasColumnName("QuiosqueId")
            .HasComment("FK para o quiosque reservado (opcional)");

        builder.Property(r => r.NomeVisitante)
            .HasColumnName("NomeVisitante")
            .HasComment("Nome completo do visitante")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Email)
            .HasColumnName("Email")
            .HasComment("E-mail do visitante")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Cpf)
            .HasColumnName("Cpf")
            .HasComment("CPF do visitante")
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(r => r.CidadeOrigem)
            .HasColumnName("CidadeOrigem")
            .HasComment("Cidade de origem do visitante")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.UfOrigem)
            .HasColumnName("UfOrigem")
            .HasComment("UF de origem do visitante")
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(r => r.Tipo)
            .HasColumnName("Tipo")
            .HasComment("Tipo da reserva (day_use, pernoite)")
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(r => r.Data)
            .HasColumnName("Data")
            .HasComment("Data de início da reserva");

        builder.Property(r => r.DataFim)
            .HasColumnName("DataFim")
            .HasComment("Data de fim da reserva (para pernoite)");

        builder.Property(r => r.QuantidadePessoas)
            .HasColumnName("QuantidadePessoas")
            .HasComment("Quantidade de pessoas na reserva");

        builder.Property(r => r.Status)
            .HasColumnName("Status")
            .HasComment("Status da reserva (confirmada, cancelada, utilizada)")
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(r => r.Token)
            .HasColumnName("Token")
            .HasComment("Token único para validação da reserva")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasComment("Data de criação do registro");

        // Indexes
        builder.HasIndex(r => r.Token)
            .HasDatabaseName("IX_Reservas_Token");

        builder.HasIndex(r => r.AtrativoId)
            .HasDatabaseName("IX_Reservas_AtrativoId");

        // Relationships
        builder.HasOne(r => r.Atrativo)
            .WithMany(a => a.Reservas)
            .HasForeignKey(r => r.AtrativoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Quiosque)
            .WithMany()
            .HasForeignKey(r => r.QuiosqueId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
