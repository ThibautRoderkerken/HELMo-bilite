using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HELMo_bilite.Models;
using Microsoft.AspNetCore.Identity;

namespace HELMo_bilite.Models
{
    public class HELMoBiliteDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public HELMoBiliteDbContext(DbContextOptions<HELMoBiliteDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Ajoutez ici les configurations supplémentaires pour vos entités
            builder.Entity<Client>().ToTable("Clients");
            builder.Entity<Admin>().ToTable("Admins");
            builder.Entity<Driver>().ToTable("Drivers");
            builder.Entity<Dispatcher>().ToTable("Dispatchers");

            // Obliger d'ajouter ces lignes sinon on risque d'avoir une erreur si deux livraisons ont la même adresse de chargement ou de déchargement
            builder.Entity<Delivery>()
                .HasOne(d => d.LoadingLocation)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Delivery>()
                .HasOne(d => d.UnloadingLocation)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Dispatcher> Dispatchers { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Truck> Trucks { get; set; }
        public DbSet<DriverLicense> DriverLicenses { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Address> Addresses { get; set; }
    }
}
