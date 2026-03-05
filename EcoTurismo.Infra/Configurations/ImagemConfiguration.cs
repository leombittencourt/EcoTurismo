using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class ImagemConfiguration : IEntityTypeConfiguration<Imagem>
{
    public void Configure(EntityTypeBuilder<Imagem> builder)
    {
        builder.ToTable("Imagens");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("Id")
            .HasComment("Identificador único da imagem");

        builder.Property(i => i.EntidadeTipo)
            .HasColumnName("EntidadeTipo")
            .HasComment("Tipo da entidade que possui esta imagem (ex: Banner, Atrativo, Municipio)")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.EntidadeId)
            .HasColumnName("EntidadeId")
            .HasComment("ID da entidade que possui esta imagem")
            .IsRequired();

        builder.Property(i => i.Categoria)
            .HasColumnName("Categoria")
            .HasComment("Categoria da imagem (principal, thumbnail, galeria, logo_login, logo_publico)")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.ImagemUrl)
            .HasColumnName("ImagemUrl")
            .HasComment("URI ou caminho da imagem (base64, URL de OCI, S3, etc)")
            .IsRequired();

        builder.Property(i => i.ThumbnailUrl)
            .HasColumnName("ThumbnailUrl")
            .HasComment("URI ou caminho da versão redimensionada (thumbnail/preview)");

        builder.Property(i => i.StorageProvider)
            .HasColumnName("StorageProvider")
            .HasComment("Provedor de armazenamento (base64, oci, s3, azure) - Opcional para compatibilidade")
            .HasMaxLength(20);

        builder.Property(i => i.Ordem)
            .HasColumnName("Ordem")
            .HasComment("Ordem de exibição (para galerias)")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(i => i.MetadadosJson)
            .HasColumnName("MetadadosJson")
            .HasComment("Metadados da imagem em JSON (nome, tamanho, dimensões, etc)")
            .IsRequired()
            .HasDefaultValue("{}");

        builder.Property(i => i.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasComment("Data de criação do registro");

        builder.Property(i => i.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasComment("Data da última atualização do registro");

        // Indexes
        builder.HasIndex(i => new { i.EntidadeTipo, i.EntidadeId })
            .HasDatabaseName("IX_Imagens_Entidade");

        builder.HasIndex(i => new { i.EntidadeTipo, i.EntidadeId, i.Categoria })
            .HasDatabaseName("IX_Imagens_Entidade_Categoria");

        builder.HasIndex(i => i.CreatedAt)
            .HasDatabaseName("IX_Imagens_CreatedAt");
    }
}
