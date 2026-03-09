using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class AuditoriaAcaoQuiosqueConfiguration : IEntityTypeConfiguration<AuditoriaAcaoQuiosque>
{
    public void Configure(EntityTypeBuilder<AuditoriaAcaoQuiosque> builder)
    {
        builder.ToTable("AuditoriasAcoesQuiosques");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("Id")
            .HasComment("Identificador unico da auditoria de acao administrativa em quiosque");

        builder.Property(a => a.QuiosqueId)
            .HasColumnName("QuiosqueId")
            .HasComment("Quiosque alvo da acao administrativa");

        builder.Property(a => a.UsuarioId)
            .HasColumnName("UsuarioId")
            .HasComment("Usuario autenticado que executou a acao");

        builder.Property(a => a.UsuarioNome)
            .HasColumnName("UsuarioNome")
            .HasMaxLength(200)
            .HasComment("Nome do usuario no momento da acao");

        builder.Property(a => a.UsuarioRole)
            .HasColumnName("UsuarioRole")
            .HasMaxLength(50)
            .HasComment("Role do usuario no momento da acao");

        builder.Property(a => a.Acao)
            .HasColumnName("Acao")
            .HasMaxLength(50)
            .IsRequired()
            .HasComment("Acao administrativa executada");

        builder.Property(a => a.Motivo)
            .HasColumnName("Motivo")
            .HasMaxLength(1000)
            .IsRequired()
            .HasComment("Motivo informado para auditoria");

        builder.Property(a => a.Payload)
            .HasColumnName("Payload")
            .HasColumnType("text")
            .HasComment("Snapshot resumido antes/depois da operacao");

        builder.Property(a => a.ReservasAfetadas)
            .HasColumnName("ReservasAfetadas")
            .HasComment("Quantidade de reservas afetadas pela acao");

        builder.Property(a => a.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasComment("Data/hora da execucao");

        builder.HasIndex(a => a.QuiosqueId)
            .HasDatabaseName("IX_AuditoriasAcoesQuiosques_QuiosqueId");

        builder.HasIndex(a => a.CreatedAt)
            .HasDatabaseName("IX_AuditoriasAcoesQuiosques_CreatedAt");

        builder.HasOne(a => a.Quiosque)
            .WithMany(q => q.AuditoriasAcoes)
            .HasForeignKey(a => a.QuiosqueId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.Usuario)
            .WithMany()
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
