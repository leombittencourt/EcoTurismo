using EcoTurismo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoTurismo.Infra.Data;

public class EcoTurismoDbContext : DbContext
{
    public EcoTurismoDbContext(DbContextOptions<EcoTurismoDbContext> options)
        : base(options) { }

    public DbSet<Municipio> Municipios => Set<Municipio>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Atrativo> Atrativos => Set<Atrativo>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Validacao> Validacoes => Set<Validacao>();
    public DbSet<Quiosque> Quiosques => Set<Quiosque>();
    public DbSet<Banner> Banners => Set<Banner>();
    public DbSet<ConfiguracaoSistema> Configuracoes => Set<ConfiguracaoSistema>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Imagem> Imagens => Set<Imagem>();
    public DbSet<AuditoriaAcaoQuiosque> AuditoriasAcoesQuiosques => Set<AuditoriaAcaoQuiosque>();
    public DbSet<AuditoriaStatusReserva> AuditoriasStatusReservas => Set<AuditoriaStatusReserva>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EcoTurismoDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var prop = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");
            if (prop != null)
                prop.CurrentValue = DateTimeOffset.UtcNow;
        }
    }
}
