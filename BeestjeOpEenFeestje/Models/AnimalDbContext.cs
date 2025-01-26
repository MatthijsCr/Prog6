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

            builder.Entity<Animal>().HasData(
               // Jungle Animals
               new Animal { Id = 1, Name = "Aap", Type = "Jungle", Price = 100.00, ImageURL = "/images/Monkey.png" },
               new Animal { Id = 2, Name = "Olifant", Type = "Jungle", Price = 200.00, ImageURL = "/images/Elephant.png" },
               new Animal { Id = 3, Name = "Zebra", Type = "Jungle", Price = 150.00, ImageURL = "/images/Zebra.png" },
               new Animal { Id = 4, Name = "Leeuw", Type = "Jungle", Price = 300.00, ImageURL = "/images/Lion.png" },

               // Farm Animals
               new Animal { Id = 5, Name = "Hond", Type = "Boerderij", Price = 50.00, ImageURL = "/images/Dog.png" },
               new Animal { Id = 6, Name = "Ezel", Type = "Boerderij", Price = 75.00, ImageURL = "/images/Donkey.png" },
               new Animal { Id = 7, Name = "Koe", Type = "Boerderij", Price = 125.00, ImageURL = "/images/Cow.png" },
               new Animal { Id = 8, Name = "Eend", Type = "Boerderij", Price = 30.00, ImageURL = "/images/Duck.png" },
               new Animal { Id = 9, Name = "Kuiken", Type = "Boerderij", Price = 10.00, ImageURL = "/images/Chicken.png" },

               // Snow Animals
               new Animal { Id = 10, Name = "Pinguïn", Type = "Sneeuw", Price = 80.00, ImageURL = "/images/Pinquin.png" },
               new Animal { Id = 11, Name = "IJsbeer", Type = "Sneeuw", Price = 250.00, ImageURL = "/images/PolarBear.png" },
               new Animal { Id = 12, Name = "Zeehond", Type = "Sneeuw", Price = 100.00, ImageURL = "/images/SeaLion.png" },

               // Desert Animals
               new Animal { Id = 13, Name = "Kameel", Type = "Woestijn", Price = 120.00, ImageURL = "/images/Camel.png" },
               new Animal { Id = 14, Name = "Slang", Type = "Woestijn", Price = 90.00, ImageURL = "/images/Snake.png" },

               // VIP Animals
               new Animal { Id = 15, Name = "T-Rex", Type = "VIP", Price = 1000.00, ImageURL = "/images/TRex.png" },
               new Animal { Id = 16, Name = "Unicorn", Type = "VIP", Price = 1500.00, ImageURL = "/images/Unicorn.png" }
           );

        }
    }
}
