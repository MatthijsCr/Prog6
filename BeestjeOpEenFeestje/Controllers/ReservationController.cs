using BeestjeOpEenFeestje.Models;
using BeestjeOpEenFeestje.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BeestjeOpEenFeestje.Controllers
{
    public class ReservationController(AnimalDbContext context) : Controller
    {
        private readonly AnimalDbContext _context = context;

        [HttpGet]
        public IActionResult Date()
        {
            return View();
        }

        [HttpPost]
        [Route("Reserveren/Beestjes")]
        public IActionResult Step1(DateOnly date)
        {
            ReservationModel viewModel = new ReservationModel
            {
                Reservation = new Reservation() { Date = date },
                Animals = _context.Animals.ToList(),
                SelectedAnimals = new(),
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Step2(ReservationModel viewModel)
        {
            List<Animal> animals = new List<Animal>();
            foreach (string animal in viewModel.SelectedAnimals)
            {
                Animal a = _context.Animals.Where(a => a.Name.ToLower().Equals(animal.ToLower())).FirstOrDefault() 
                    ?? throw new Exception("Beestje " + animal + " bestaat niet.");
                animals.Add(a);
            }


            _context.SaveChanges();

            return View(viewModel);
        }
    }
}
