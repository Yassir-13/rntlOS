using Microsoft.EntityFrameworkCore;
using rntlOS.Core.Models;

namespace rntlOS.Core.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<TypeVehicule> TypesVehicules { get; set; }
        public DbSet<Vehicule> Vehicules { get; set; }
        public DbSet<VehiculeImage> VehiculeImages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Paiement> Paiements { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Exemple : empêcher la suppression en cascade d’un TypeVehicule
            modelBuilder.Entity<Vehicule>()
                .HasOne(v => v.TypeVehicule)
                .WithMany()
                .HasForeignKey(v => v.TypeVehiculeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
