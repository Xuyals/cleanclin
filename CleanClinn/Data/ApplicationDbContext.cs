using Microsoft.EntityFrameworkCore;
using CleanClinn.Models;

namespace CleanClinn.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Trabajador> Trabajadores { get; set; }
        public DbSet<TipoServicio> TiposServicio { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed de tipos de servicio
            modelBuilder.Entity<TipoServicio>().HasData(
                new TipoServicio { Id = 1, Nombre = "Limpieza", Descripcion = "Limpieza general del hogar", PrecioBase = 50000 },
                new TipoServicio { Id = 2, Nombre = "Plomería", Descripcion = "Reparación de tuberías y filtraciones", PrecioBase = 80000 },
                new TipoServicio { Id = 3, Nombre = "Electricidad", Descripcion = "Instalaciones y reparaciones eléctricas", PrecioBase = 90000 },
                new TipoServicio { Id = 4, Nombre = "Gas", Descripcion = "Mantenimiento de instalaciones de gas", PrecioBase = 100000 },
                new TipoServicio { Id = 5, Nombre = "Vidrios", Descripcion = "Reparación y cambio de vidrios", PrecioBase = 70000 },
                new TipoServicio { Id = 6, Nombre = "Reparaciones Generales", Descripcion = "Reparaciones varias del hogar", PrecioBase = 60000 }
            );

            // Evitar ciclos en la serialización
            modelBuilder.Entity<Solicitud>()
                .HasOne(s => s.Trabajador)
                .WithMany()
                .HasForeignKey(s => s.TrabajadorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}