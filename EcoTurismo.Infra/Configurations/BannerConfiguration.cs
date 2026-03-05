using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoTurismo.Infra.Configurations;

public class BannerConfiguration : IEntityTypeConfiguration<Banner>
{
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
        builder.ToTable("Banners");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("Id")
            .HasComment("Identificador único do banner");

        builder.Property(b => b.MunicipioId)
            .HasColumnName("MunicipioId")
            .HasComment("FK para o município (opcional)");

        builder.Property(b => b.ImagemId)
            .HasColumnName("ImagemId")
            .HasComment("FK para a imagem do banner");

        builder.Property(b => b.Titulo)
            .HasColumnName("Titulo")
            .HasComment("Título exibido no banner");

        builder.Property(b => b.Subtitulo)
            .HasColumnName("Subtitulo")
            .HasComment("Subtítulo exibido no banner");

        builder.Property(b => b.Link)
            .HasColumnName("Link")
            .HasComment("Link de redirecionamento ao clicar no banner");

        builder.Property(b => b.Ordem)
            .HasColumnName("Ordem")
            .HasComment("Ordem de exibição do banner");

        builder.Property(b => b.Ativo)
            .HasColumnName("Ativo")
            .HasComment("Indica se o banner está ativo para exibição");

        builder.Property(b => b.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasComment("Data de criação do registro");

        builder.Property(b => b.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasComment("Data da última atualização do registro");

        // Indexes
        builder.HasIndex(b => b.MunicipioId)
            .HasDatabaseName("IX_Banners_MunicipioId");

        builder.HasIndex(b => b.ImagemId)
            .HasDatabaseName("IX_Banners_ImagemId");

        builder.HasIndex(b => b.Ordem)
            .HasDatabaseName("IX_Banners_Ordem");

        // Relationships
        builder.HasOne(b => b.Municipio)
            .WithMany(m => m.Banners)
            .HasForeignKey(b => b.MunicipioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Imagem)
            .WithMany()
            .HasForeignKey(b => b.ImagemId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
