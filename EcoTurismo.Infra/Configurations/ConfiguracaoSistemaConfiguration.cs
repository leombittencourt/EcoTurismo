using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class ConfiguracaoSistemaConfiguration : IEntityTypeConfiguration<ConfiguracaoSistema>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoSistema> builder)
    {
        builder.ToTable("configuracoes_sistema");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .HasComment("Identificador único da configuração");

        builder.Property(c => c.Chave)
            .HasColumnName("chave")
            .HasComment("Chave única da configuração")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Valor)
            .HasColumnName("valor")
            .HasComment("Valor da configuração");

        builder.Property(c => c.Descricao)
            .HasColumnName("descricao")
            .HasComment("Descrição da finalidade da configuração");

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .HasComment("Data da última atualização do registro");

        // Indexes
        builder.HasIndex(c => c.Chave)
            .IsUnique();
    }
}
