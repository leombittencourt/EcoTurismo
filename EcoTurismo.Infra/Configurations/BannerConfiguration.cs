using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class BannerConfiguration : IEntityTypeConfiguration<Banner>
{
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
        builder.ToTable("banners");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("id")
            .HasComment("Identificador único do banner");

        builder.Property(b => b.Titulo)
            .HasColumnName("titulo")
            .HasComment("Título exibido no banner");

        builder.Property(b => b.Subtitulo)
            .HasColumnName("subtitulo")
            .HasComment("Subtítulo exibido no banner");

        builder.Property(b => b.ImagemUrl)
            .HasColumnName("imagem_url")
            .HasComment("URL da imagem do banner")
            .IsRequired();

        builder.Property(b => b.Link)
            .HasColumnName("link")
            .HasComment("Link de redirecionamento ao clicar no banner");

        builder.Property(b => b.Ordem)
            .HasColumnName("ordem")
            .HasComment("Ordem de exibição do banner");

        builder.Property(b => b.Ativo)
            .HasColumnName("ativo")
            .HasComment("Indica se o banner está ativo para exibição");

        builder.Property(b => b.CreatedAt)
            .HasColumnName("created_at")
            .HasComment("Data de criação do registro");

        builder.Property(b => b.UpdatedAt)
            .HasColumnName("updated_at")
            .HasComment("Data da última atualização do registro");
    }
}
