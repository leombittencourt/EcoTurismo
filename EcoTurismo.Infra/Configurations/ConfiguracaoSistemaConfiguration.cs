using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class ConfiguracaoSistemaConfiguration : IEntityTypeConfiguration<ConfiguracaoSistema>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoSistema> builder)
    {
        builder.ToTable("ConfiguracoesSistema");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("Id")
            .HasComment("Identificador único da configuração");

        builder.Property(c => c.Chave)
            .HasColumnName("Chave")
            .HasComment("Chave única da configuração")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Valor)
            .HasColumnName("Valor")
            .HasComment("Valor da configuração");

        builder.Property(c => c.Descricao)
            .HasColumnName("Descricao")
            .HasComment("Descrição da finalidade da configuração");

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasComment("Data da última atualização do registro");

        // Indexes
        builder.HasIndex(c => c.Chave)
            .IsUnique();
    }
}
