using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("Id")
            .HasComment("Identificador único do usuário");

        builder.Property(u => u.Nome)
            .HasColumnName("Nome")
            .HasComment("Nome completo do usuário")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .HasColumnName("Email")
            .HasComment("Endereço de e-mail do usuário")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.PasswordHash)
            .HasColumnName("PasswordHash")
            .HasComment("Hash da senha do usuário")
            .IsRequired();

        builder.Property(u => u.RoleId)
            .HasColumnName("RoleId")
            .HasComment("FK para a role do usuário");

        builder.Property(u => u.MunicipioId)
            .HasColumnName("MunicipioId")
            .HasComment("FK para o município vinculado");

        builder.Property(u => u.AtrativoId)
            .HasColumnName("AtrativoId")
            .HasComment("FK para o atrativo vinculado");

        builder.Property(u => u.Telefone)
            .HasColumnName("Telefone")
            .HasComment("Telefone do usuário")
            .HasMaxLength(20);

        builder.Property(u => u.Cpf)
            .HasColumnName("Cpf")
            .HasComment("CPF do usuário")
            .HasMaxLength(14);

        builder.Property(u => u.Ativo)
            .HasColumnName("Ativo")
            .HasComment("Indica se o usuário está ativo no sistema")
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasComment("Data de criação do registro");

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasComment("Data da última atualização do registro");

        // Indexes
        builder.HasIndex(u => u.Email)
            .HasDatabaseName("IX_Usuarios_Email")
            .IsUnique();

        builder.HasIndex(u => u.Cpf)
            .HasDatabaseName("IX_Usuarios_Cpf")
            .IsUnique()
            .HasFilter("\"Cpf\" IS NOT NULL");

        builder.HasIndex(u => u.RoleId)
            .HasDatabaseName("IX_Usuarios_RoleId");

        // Relationships
        builder.HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Municipio)
            .WithMany()
            .HasForeignKey(u => u.MunicipioId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(u => u.Atrativo)
            .WithMany()
            .HasForeignKey(u => u.AtrativoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
