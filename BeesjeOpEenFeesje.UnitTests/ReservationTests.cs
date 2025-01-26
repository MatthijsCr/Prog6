using BeestjeOpEenFeestje.Controllers;
using BeestjeOpEenFeestje.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeesjeOpEenFeesje.UnitTests
{
    [TestClass]
    public class ReservationTests
    {
        private Mock<SignInManager<AppUser>> _signInManagerMock;
        private Mock<UserManager<AppUser>> _userManagerMock;
        private AnimalDbContext _dbContext;
        private ReservationController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AnimalDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Unique name for the in-memory DB
                .Options;

            _dbContext = new AnimalDbContext(options);

            _dbContext.Animals.Add(new Animal { Id = 1, Name = "Lion", Type = "Wild", Price = 100, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" });
            _dbContext.Animals.Add(new Animal { Id = 2, Name = "Penguin", Type = "Arctic", Price = 50, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" });
            _dbContext.SaveChanges();

            _userManagerMock = new Mock<UserManager<AppUser>>(
                Mock.Of<IUserStore<AppUser>>(),
                null, null, null, null, null, null, null, null
            );

            _signInManagerMock = new Mock<SignInManager<AppUser>>(
                _userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(),
                null, null, null, null
            );

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                            .ReturnsAsync(new AppUser { CustomerCard = CustomerCardType.Geen });

            _controller = new ReservationController(_dbContext, _signInManagerMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_FarmAnimalAndLion_ReturnsFalse()
        {
            var animals = new List<Animal>
            {
                new Animal { Id = 1, Name = "leeuw", Type = "", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 2, Name = "leeuw", Type = "Boerderij", Price = 10.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
            };

            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 30));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_FarmAnimalAndLionWithWeirdCaps_ReturnsFalse()
        {
            var animals = new List<Animal>
            {
                new Animal { Id = 1, Name = "LeEuW", Type = "", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 2, Name = "KoE", Type = "BoeRdeRIJ", Price = 10.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
            };

            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 30));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_WeekendAndPinquin_ReturnsFalse()
        {
            var animals = new List<Animal>
            {
                new Animal { Id = 1, Name = "pinguin", Type = "Dier", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            };

            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26)); // weekend

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_DesertAndCold_ReturnsFalse()
        {
            var animals = new List<Animal>
            {
                new Animal { Id = 1, Name = "warmDier", Type = "Woestijn", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            };

            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_SnowAndWarm_ReturnsFalse()
        {
            var animals = new List<Animal>
            {
                new Animal { Id = 1, Name = "koudDier", Type = "Sneeuw", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            };

            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 08, 26));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_ConditionsAreRightAndHasAccesToMoreAnimals_ReturnsTrue()
        {
            var animals = new List<Animal>
            {
                new Animal { Id = 3, Name = "Koe", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 4, Name = "Koe", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 5, Name = "Koe", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 6, Name = "Koe", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 7, Name = "Koe", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 8, Name = "Koe", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 9, Name = "Koe", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 10, Name = "Koe", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
            };

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                            .ReturnsAsync(new AppUser { CustomerCard = CustomerCardType.Goud });
            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 27)); 

            Assert.IsTrue(result, "Animals should be allowed since all conditions pass.");
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_UserHasNoCardAnd4AnimalsAndNotWednesday_ReturnsFalse()
        {
            var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "Koe", Type = "Wild", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 2, Name = "Geit", Type = "Arctic", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 3, Name = "Varken", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 4, Name = "Duck", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
        };
            var user = new AppUser { CustomerCard = CustomerCardType.Geen };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);

            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26)); // Not Wednesday

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_UserHasNoCardAnd4AnimalsAndWednesday_ReturnsTrue()
        {
            var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "Koe", Type = "Wild", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 2, Name = "Geit", Type = "Arctic", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 3, Name = "Varken", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 4, Name = "Duck", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
        };
            var user = new AppUser { CustomerCard = CustomerCardType.Geen };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);

            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 29)); // Wednesday

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_UserHasNoCardAndMoreThan4AnimalsAndWednesday_ReturnsFalse()
        {
            var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "Koe", Type = "boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 2, Name = "Geit", Type = "Arctic", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 3, Name = "Varken", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 4, Name = "Kip", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 5, Name = "Eend", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
        };
            var user = new AppUser { CustomerCard = CustomerCardType.Geen };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);

            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 29)); // Wednesday

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_UserHasZilverCardAndMoreThan4Animals_ReturnsFalse()
        {
            var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "Koe", Type = "Wild", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 2, Name = "Geit", Type = "Arctic", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 3, Name = "Varken", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 4, Name = "Kip", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 5, Name = "Eend", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
        };
            var user = new AppUser { CustomerCard = CustomerCardType.Zilver };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);

            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_UserHasZilverCardAnd4Animals_ReturnsTrue()
        {
            var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "Koe", Type = "Wild", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 2, Name = "Geit", Type = "Arctic", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 3, Name = "Varken", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 4, Name = "Eend", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
        };
            var user = new AppUser { CustomerCard = CustomerCardType.Zilver };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);

            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_UserHasPlatinumCardAndVipAnimals_ReturnsTrue()
        {
            var animals = new List<Animal>
            {
                new Animal { Id = 1, Name = "VIP Lion", Type = "VIP", Price = 100.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
            };
            var user = new AppUser { CustomerCard = CustomerCardType.Platinum };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);

            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_NoUserAndMoreThan3Animals_ReturnsFalse()
        {
            var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "Koe", Type = "Wild", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 2, Name = "Geit", Type = "Arctic", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 3, Name = "Varken", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 4, Name = "Eend", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
        };

            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_NoUserAndVipAnimals_ReturnsFalse()
        {
            var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "VIP Penguin", Type = "VIP", Price = 50.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
        };

            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_GoldCardAndVipAnimals_ReturnsFalse()
        {
            var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "VIP Penguin", Type = "VIP", Price = 50.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
        };
            var user = new AppUser { CustomerCard = CustomerCardType.Goud };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);

            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task CalculateDiscount_ShouldApplyMondayOrTuesdayDiscount()
        {
            DateOnly date = new DateOnly(2025, 1, 27);
            List<Animal> animals = new List<Animal> { new Animal { Id = 1, Name = "dier", Type = "diertje", Price = 50.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }, };

            var discounts = await _controller.CalculateDiscount(animals, date);

            Assert.IsTrue(discounts.Any(d => d.Amount == 15), "Monday or Tuesday discount should be applied.");
        }

        [TestMethod]
        public async Task CalculateDiscount_ShouldApplyCustomerCardDiscount()
        {
            var user = new AppUser { CustomerCard = CustomerCardType.Goud };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);

            DateOnly date = new DateOnly(2025, 1, 26);
            List<Animal> animals = new List<Animal> {new Animal { Id = 1, Name = "dier", Type = "diertje", Price = 50.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            };

            var discounts = await _controller.CalculateDiscount(animals, date);

            Assert.IsTrue(discounts.Any(d => d.Amount == 10), "Customer card discount should be applied.");
        }

        [TestMethod]
        public async Task CalculateDiscount_ShouldApplyThreeSameTypeDiscount()
        {
            DateOnly date = new DateOnly(2025, 1, 26);
            List<Animal> animals = new List<Animal>
            {
            new Animal { Id = 1, Name = "dier", Type = "diertje", Price = 50.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 1, Name = "dier", Type = "diertje", Price = 50.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 1, Name = "dier", Type = "diertje", Price = 50.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
            };

            var discounts = await _controller.CalculateDiscount(animals, date);

            Assert.IsTrue(discounts.Any(d => d.Amount == 10), "Three same type discount should be applied.");
        }

        [TestMethod]
        public async Task CalculateDiscount_ShouldApplyLetterComboDiscount()
        {
            DateOnly date = new DateOnly(2025, 1, 26);
            List<Animal> animals = new List<Animal> { new Animal { Id = 1, Name = "Alpha", Type = "diertje", Price = 50.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }, };

            var discounts = await _controller.CalculateDiscount(animals, date);

            Assert.IsTrue(discounts.Any(d => d.Amount == 2), "Letter combo discount should be applied for letter 'A'.");
        }

        [TestMethod]
        public async Task CalculateDiscount_ShouldNotExceedMaxDiscount()
        {
            DateOnly date = new DateOnly(2025, 1, 27);
            List<Animal> animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "dier", Type = "diertje", Price = 50.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },

            new Animal { Id = 1, Name = "abcdefghijklmno", Type = "diertje", Price = 50.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },

            new Animal {Id = 1, Name = "dier", Type = "diertje", Price = 50.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png"}
        };

            /* Kortingen
             * ma: 15%
             * 3 types: 10%
             * letter combo: 15 * 2 = 30
             * 
             * Totaal: 65 */

            var discounts = await _controller.CalculateDiscount(animals, date);

            Assert.IsTrue(discounts.Select(d => d.Amount).Sum() <= 60, "Discount total should not exceed 60%.");
        }
    }
}
