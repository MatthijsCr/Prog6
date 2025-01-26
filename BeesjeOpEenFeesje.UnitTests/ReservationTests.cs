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
            // Create options for AnimalDbContext to use an in-memory database
            var options = new DbContextOptionsBuilder<AnimalDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Unique name for the in-memory DB
                .Options;

            // Initialize the real AnimalDbContext with the in-memory database
            _dbContext = new AnimalDbContext(options);

            // Add sample data to the in-memory database
            _dbContext.Animals.Add(new Animal { Id = 1, Name = "Lion", Type = "Wild", Price = 100, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" });
            _dbContext.Animals.Add(new Animal { Id = 2, Name = "Penguin", Type = "Arctic", Price = 50, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" });
            _dbContext.SaveChanges();

            // Mock UserManager
            _userManagerMock = new Mock<UserManager<AppUser>>(
                Mock.Of<IUserStore<AppUser>>(),
                null, null, null, null, null, null, null, null
            );

            // Mock SignInManager and inject the mocked UserManager
            _signInManagerMock = new Mock<SignInManager<AppUser>>(
                _userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(),
                null, null, null, null
            );

            // Mock GetUserAsync to return a predefined AppUser based on your needs
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                            .ReturnsAsync(new AppUser { CustomerCard = CustomerCardType.Geen }); // Mock for "Geen" user card

            // Initialize the controller with the real DbContext and mocked SignInManager
            _controller = new ReservationController(_dbContext, _signInManagerMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Ensure the in-memory database is deleted after each test
            _dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_FarmAnimalAndLion_ReturnsFalse()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Animal { Id = 1, Name = "leeuw", Type = "", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 2, Name = "leeuw", Type = "Boerderij", Price = 10.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
            };

            // Act
            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 30)); // Sunday

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_FarmAnimalAndLionWithWeirdCaps_ReturnsFalse()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Animal { Id = 1, Name = "LeEuW", Type = "", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 2, Name = "KoE", Type = "BoeRdeRIJ", Price = 10.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
            };

            // Act
            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 30)); // Sunday

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_SundayAndPinquin_ReturnsFalse()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Animal { Id = 1, Name = "pinguin", Type = "Dier", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            };

            // Act
            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26)); // Sunday

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_DesertAndCold_ReturnsFalse()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Animal { Id = 1, Name = "warmDier", Type = "Woestijn", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            };

            // Act
            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26)); // Sunday

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_SnowAndWarm_ReturnsFalse()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Animal { Id = 1, Name = "koudDier", Type = "Sneeuw", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            };

            // Act
            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 08, 26)); // Sunday

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_ConditionsAreRightAndHasAccesToMoreAnimals_ReturnsTrue()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Animal { Id = 3, Name = "Cow", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 4, Name = "Cow", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 5, Name = "Cow", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 6, Name = "Cow", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 7, Name = "Cow", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 8, Name = "Cow", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 9, Name = "Cow", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 10, Name = "Cow", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
            };

            // Mock a user with a Platinum card
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                            .ReturnsAsync(new AppUser { CustomerCard = CustomerCardType.Goud });

            // Act
            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 27)); // Monday

            // Assert
            Assert.IsTrue(result, "Animals should be allowed since all conditions pass.");
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_UserHasNoCardAnd4AnimalsAndNotWednesday_ReturnsFalse()
        {
            // Arrange
            var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "Lion", Type = "Wild", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 2, Name = "Penguin", Type = "Arctic", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 3, Name = "Cow", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 4, Name = "Duck", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
        };
            var user = new AppUser { CustomerCard = CustomerCardType.Geen };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);

            // Act
            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26)); // Not Wednesday

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_UserHasNoCardAnd4AnimalsAndWednesday_ReturnsTrue()
        {
            // Arrange
            var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "Lion", Type = "Wild", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 2, Name = "Penguin", Type = "Arctic", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 3, Name = "Cow", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 4, Name = "Duck", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
        };
            var user = new AppUser { CustomerCard = CustomerCardType.Geen };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);

            // Act
            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 29)); // Wednesday

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_UserHasNoCardAndMoreThan4AnimalsAndWednesday_ReturnsFalse()
        {
            // Arrange
            var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "Lion", Type = "boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 2, Name = "Penguin", Type = "Arctic", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 3, Name = "Cow", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 4, Name = "Duck", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 5, Name = "Eend", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
        };
            var user = new AppUser { CustomerCard = CustomerCardType.Geen };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);

            // Act
            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 29)); // Wednesday

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_ReturnsFalse_WhenUserHasZilverCardAndMoreThan4Animals()
        {
            // Arrange
            var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "Lion", Type = "Wild", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 2, Name = "Penguin", Type = "Arctic", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 3, Name = "Cow", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 4, Name = "Duck", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 5, Name = "Sheep", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
        };
            var user = new AppUser { CustomerCard = CustomerCardType.Zilver };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);

            // Act
            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26)); // Not Wednesday

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_UserHasPlatinumCardAndVipAnimals_ReturnsTrue()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Animal { Id = 1, Name = "VIP Lion", Type = "VIP", Price = 100.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
            };
            var user = new AppUser { CustomerCard = CustomerCardType.Platinum };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(user);

            // Act
            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26)); // Monday

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_NoUserAndMoreThan3Animals_ReturnsFalse()
        {
            // Arrange
            var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "Lion", Type = "Wild", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 2, Name = "Penguin", Type = "Arctic", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 3, Name = "Cow", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
            new Animal { Id = 4, Name = "Duck", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
        };

            // Act
            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26)); // Not Wednesday

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AreAnimalsAllowed_NoUserAndVipAnimals_ReturnsFalse()
        {
            // Arrange
            var animals = new List<Animal>
        {
            new Animal { Id = 1, Name = "VIP Penguin", Type = "VIP", Price = 50.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
        };

            // Act
            var result = await _controller.AreAnimalsAllowed(animals, new DateOnly(2025, 01, 26)); // Not Wednesday

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task CalculateDiscount_ReturnsCorrectDiscounts()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Animal { Id = 4, Name = "Duck", Type = "Boerderij", Price = 5.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 5, Name = "Cow", Type = "Boerderij", Price = 15.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 6, Name = "Sheep", Type = "Boerderij", Price = 10.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
            };

            // Mock a user with a Zilver card
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                            .ReturnsAsync(new AppUser { CustomerCard = CustomerCardType.Zilver });

            // Act
            var discounts = await _controller.CalculateDiscount(animals, new DateOnly(2025, 01, 29)); // Wednesday

            // Assert
            Assert.AreEqual(2, discounts.Count, "Two discounts should be applied (Customer Card and Three Same Type).");
            Assert.IsTrue(discounts.Any(d => d.Amount == 10), "A 10% discount should be applied for the customer card.");
            Assert.IsTrue(discounts.Any(d => d.Amount == 10), "A 10% discount should be applied for three animals of the same type.");
        }

        [TestMethod]
        public async Task CalculateDiscount_CapsDiscountAtMaximum()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Animal { Id = 7, Name = "Duck", Type = "Boerderij", Price = 10.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 8, Name = "Penguin", Type = "Boerderij", Price = 20.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 9, Name = "Cow", Type = "Boerderij", Price = 30.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" },
                new Animal { Id = 10, Name = "Sheep", Type = "Boerderij", Price = 40.0, ImageURL = "PlaatjesMakenNiksUitVoorDit.png" }
            };

            // Mock a user with a Platinum card
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                            .ReturnsAsync(new AppUser { CustomerCard = CustomerCardType.Platinum });

            // Act
            var discounts = await _controller.CalculateDiscount(animals, new DateOnly(2025, 01, 26)); // Monday

            // Assert
            var totalDiscount = discounts.Sum(d => d.Amount);
            Assert.IsTrue(totalDiscount <= 60, "Total discount should be capped at 60%.");
        }
    }
}
