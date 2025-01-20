using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BeestjeOpEenFeestje.Models
{
    public class AnimalDbContext : IdentityDbContext<AppUser>
    {
        public AnimalDbContext(DbContextOptions<AnimalDbContext> options) : base(options)
        {
            
        }

        DbSet<Animal> Animals { get; set; }
        DbSet<Reservation> Reservations { get; set; }
    }
}
