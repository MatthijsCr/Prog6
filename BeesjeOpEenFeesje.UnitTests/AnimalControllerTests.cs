using BeestjeOpEenFeestje.Controllers;
using BeestjeOpEenFeestje.Models;
using BeestjeOpEenFeestje.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeesjeOpEenFeesje.UnitTests
{
    [TestClass]
    public class AnimalControllerTests { 

        private AnimalDbContext _context;
        private AnimalController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<AnimalDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AnimalDbContext(options);

            SeedDatabase();

            _controller = new AnimalController(_context);
        }

        private void SeedDatabase()
        {
            if (!_context.Animals.Any())
            {
                _context.Animals.AddRange(
                    new Animal { Id = 1, Name = "Lion", Type = "Wild", Price = 500, ImageURL = "/Images/lion.png" },
                    new Animal { Id = 2, Name = "Elephant", Type = "Wild", Price = 700, ImageURL = "/Images/Elephant.png" }
                );
                _context.SaveChanges();
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {   
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }


        [TestMethod]
        public void CreateAnimal_ReturnsView_WithImageUrls()
        {
            var result = _controller.CreateAnimal() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ContainsKey("ImgUrls"));
        }

        [TestMethod]
        public void CreateAnimal_Valid_ReturnsToAnimalList()
        {
            var model = new CreateAnimalModel()
            {
                Name = "AirBud",
                Type = "Wild",
                Price = 5,
                ImageURL = "/Images/Beaver.png"
            };

            var result = _controller.CreateAnimal(model) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("AnimalList", result.ActionName);
            Assert.IsTrue(_context.Animals.Any(a => a.Name == "AirBud"));
        }

        [TestMethod]
        public void CreateAnimal_InvalidPrice_ReturnsViewWithModel()
        {
            var model = new CreateAnimalModel
            {
                Name = "AirBud",
                Type = "Wild",
                Price = -5, 
                ImageURL = "/Images/Beaver.png"
            };

            var result = _controller.CreateAnimal(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.AreEqual(model, result.Model);

            Assert.IsTrue(!_context.Animals.Any(a => a.Name == "AirBud"));
        }

        [TestMethod]
        public void UpdateAnimal_Get_ReturnsViewWithCorrectModel()
        {
            var animalId = 3;
            var animal = new Animal
            {
                Id = animalId,
                Name = "AirBud",
                Type = "Wild",
                Price = 100,
                ImageURL = "/Images/Beaver.png"
            };

            _context.Animals.Add(animal);
            _context.SaveChanges();

            var result = _controller.UpdateAnimal(animalId) as ViewResult;
            var model = result?.Model as UpdateAnimalModel;

            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual(animalId, model?.Id); 
            Assert.AreEqual(animal.Name, model?.Name);
            Assert.AreEqual(animal.Price, model?.Price);
            Assert.AreEqual(animal.ImageURL, model?.ImageURL); 
            Assert.AreEqual(animal.Type, model?.Type);
        }

        [TestMethod]
        public void UpdateAnimal_Post_ValidModel_UpdatesAnimalAndRedirects()
        {
            var animalId = 3;
            var animal = new Animal
            {
                Id = animalId,
                Name = "AirBud",
                Type = "Wild",
                Price = 100,
                ImageURL = "/Images/Beaver.png"
            };

            _context.Animals.Add(animal);
            _context.SaveChanges();

            var model = new UpdateAnimalModel
            {
                Id = animalId,
                Name = "New Name",
                Type = "Domestic",
                Price = 150,
                ImageURL = "/Images/NewImage.png"
            };

            var result = _controller.UpdateAnimal(model) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("AnimalList", result.ActionName);
            var updatedAnimal = _context.Animals.FirstOrDefault(a => a.Id == animalId);
            Assert.IsNotNull(updatedAnimal);
            Assert.AreEqual("New Name", updatedAnimal.Name);
            Assert.AreEqual(150, updatedAnimal.Price);
            Assert.AreEqual("Domestic", updatedAnimal.Type);
            Assert.AreEqual("/Images/NewImage.png", updatedAnimal.ImageURL);
        }

        [TestMethod]
        public void UpdateAnimal_Post_InvalidModel_NegativePrice_ReturnsViewWithModelErrors()
        {
            var animalId = 3;
            var animal = new Animal
            {
                Id = animalId,
                Name = "AirBud",
                Type = "Wild",
                Price = 100,
                ImageURL = "/Images/Beaver.png"
            };

            _context.Animals.Add(animal);
            _context.SaveChanges();

            var model = new UpdateAnimalModel
            {
                Id = animalId,
                Name = "New Name",
                Type = "Domestic",
                Price = -50,
                ImageURL = "/Images/NewImage.png"
            };

            var result = _controller.UpdateAnimal(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState.ContainsKey("Price"));
            var unchangedAnimal = _context.Animals.FirstOrDefault(a => a.Id == animalId);
            Assert.IsNotNull(unchangedAnimal);
            Assert.AreEqual(100, unchangedAnimal.Price);
        }

        [TestMethod]
        public void UpdateAnimal_Post_InvalidModel_AnimalDoesntExist_ReturnsViewWithModelErrors()
        {
            var animalId = 3;

            var animal = new Animal
            {
                Id = animalId,
                Name = "AirBud",
                Type = "Wild",
                Price = 100,
                ImageURL = "/Images/Beaver.png"
            };

            var model = new UpdateAnimalModel
            {
                Id = animalId,
                Name = "New Name",
                Type = "Domestic",
                Price = -50,  
                ImageURL = "/Images/NewImage.png"
            };

            var result = _controller.UpdateAnimal(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
            var unchangedAnimal = _context.Animals.FirstOrDefault(a => a.Id == animalId);
            Assert.IsNull(unchangedAnimal);
        }

        [TestMethod]
        public void DeleteAnimal_ValidId_DeletesAnimalAndRedirects()
        {
            var animalId = 3;
            var animal = new Animal
            {
                Id = animalId,
                Name = "AirBud",
                Type = "Wild",
                Price = 100,
                ImageURL = "/Images/Beaver.png"
            };

            _context.Animals.Add(animal);
            _context.SaveChanges();

            var result = _controller.DeleteAnimal(animalId) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("AnimalList", result.ActionName);
            var deletedAnimal = _context.Animals.FirstOrDefault(a => a.Id == animalId);
            Assert.IsNull(deletedAnimal);
        }


    }
}