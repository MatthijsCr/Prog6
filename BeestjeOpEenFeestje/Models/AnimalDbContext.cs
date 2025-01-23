using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BeestjeOpEenFeestje.Models
{
    public class AnimalDbContext : IdentityDbContext<AppUser>
    {
        public AnimalDbContext(DbContextOptions<AnimalDbContext> options) : base(options)
        {
        }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Reservation>()
                .HasIndex(r => new { r.AnimalId, r.Date })
                .IsUnique();
        }
    }
}
