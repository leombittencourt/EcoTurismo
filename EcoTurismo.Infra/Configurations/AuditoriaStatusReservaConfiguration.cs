using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class AuditoriaStatusReservaConfiguration : IEntityTypeConfiguration<AuditoriaStatusReserva>
{
    public void Configure(EntityTypeBuilder<AuditoriaStatusReserva> builder)
    {
        builder.ToTable("AuditoriasStatusReservas");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("Id")
            .HasComment("Identificador unico da auditoria de status da reserva");

        builder.Property(a => a.ReservaId)
            .HasColumnName("ReservaId")
            .IsRequired()
            .HasComment("Reserva alterada");

        builder.Property(a => a.UsuarioId)
            .HasColumnName("UsuarioId")
            .HasComment("Usuario autenticado que executou a alteracao");

        builder.Property(a => a.UsuarioNome)
            .HasColumnName("UsuarioNome")
            .HasMaxLength(200)
            .HasComment("Nome do usuario no momento da alteracao");

        builder.Property(a => a.UsuarioRole)
            .HasColumnName("UsuarioRole")
            .HasMaxLength(50)
            .HasComment("Role do usuario no momento da alteracao");

        builder.Property(a => a.StatusAnterior)
            .HasColumnName("StatusAnterior")
            .HasMaxLength(30)
            .IsRequired()
            .HasComment("Status anterior da reserva");

        builder.Property(a => a.StatusNovo)
            .HasColumnName("StatusNovo")
            .HasMaxLength(30)
            .IsRequired()
            .HasComment("Novo status da reserva");

        builder.Property(a => a.Motivo)
            .HasColumnName("Motivo")
            .HasMaxLength(1000)
            .IsRequired()
            .HasComment("Motivo informado para alteracao de status");

        builder.Property(a => a.Payload)
            .HasColumnName("Payload")
            .HasColumnType("text")
            .HasComment("Snapshot da reserva na alteracao");

        builder.Property(a => a.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasComment("Data/hora da alteracao");

        builder.HasIndex(a => a.ReservaId)
            .HasDatabaseName("IX_AuditoriasStatusReservas_ReservaId");

        builder.HasIndex(a => a.CreatedAt)
            .HasDatabaseName("IX_AuditoriasStatusReservas_CreatedAt");

        builder.HasOne(a => a.Reserva)
            .WithMany(r => r.AuditoriasStatus)
            .HasForeignKey(a => a.ReservaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Usuario)
            .WithMany()
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
