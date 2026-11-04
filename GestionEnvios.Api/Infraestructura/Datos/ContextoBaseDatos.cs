using GestionEnvios.Api.Dominio.Modelos;
using Microsoft.EntityFrameworkCore;

namespace GestionEnvios.Api.Infraestructura.Datos;

public class ContextoBaseDatos(DbContextOptions<ContextoBaseDatos> opciones) : DbContext(opciones)
{
    public DbSet<Envio> Envios { get; set; }
    public DbSet<HistorialEnvio> HistorialEnvios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Envio>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.NumeroGuia).IsUnique();
            e.Property(x => x.PesoKilos).HasPrecision(10, 2);
            e.Property(x => x.LargoCentimetros).HasPrecision(10, 2);
            e.Property(x => x.AnchoCentimetros).HasPrecision(10, 2);
            e.Property(x => x.AltoCentimetros).HasPrecision(10, 2);
            e.Property(x => x.EtiquetaPdfBytes).HasColumnType("varbinary(max)");
        });

        modelBuilder.Entity<HistorialEnvio>(h =>
        {
            h.HasKey(x => x.Id);
            h.HasOne(x => x.Envio)
             .WithMany(x => x.Historial)
             .HasForeignKey(x => x.EnvioId);
        });
    }
}