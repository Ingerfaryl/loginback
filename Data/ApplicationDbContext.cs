using Microsoft.EntityFrameworkCore;
using login.Entities;
namespace login.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Persona>? Personas { get; set; }
        public DbSet<Perfil>? Perfiles { get; set; }
        public DbSet<Usuario>? Usuarios { get; set; }
        public DbSet<Ventana>? Ventanas { get; set; }
        public DbSet<PerfilVentana>? PerfilVentanas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar clave compuesta para PerfilVentana
            modelBuilder.Entity<PerfilVentana>()
                .HasKey(pv => new { pv.IdPerfil, pv.IdVentana });

            // Configurar relaciones
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Persona)
                .WithOne(p => p.Usuario)
                .HasForeignKey<Usuario>(u => u.IdPersona);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Perfil)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(u => u.IdPerfil);

            modelBuilder.Entity<PerfilVentana>()
                .HasOne(pv => pv.Perfil)
                .WithMany(p => p.PerfilVentanas)
                .HasForeignKey(pv => pv.IdPerfil);

            modelBuilder.Entity<PerfilVentana>()
                .HasOne(pv => pv.Ventana)
                .WithMany(v => v.PerfilVentanas)
                .HasForeignKey(pv => pv.IdVentana);
        }
    }
}
