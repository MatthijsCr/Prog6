using BeestjeOpEenFeestje.Models;
using BeestjeOpEenFeestje.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BeestjeOpEenFeestje.Controllers
{
    public class ReservationController(AnimalDbContext context) : Controller
    {
        private readonly AnimalDbContext _context = context;

        [HttpGet]
        [Route("Reserveren/Datum")]
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
                Reservation = new() { Date = date },
                Animals = _context.Animals.ToList(),
                SelectedAnimals = new(),
            };

            _context.Add(viewModel.Reservation);
            _context.SaveChanges();

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

            _context.Update(viewModel.Reservation);
            _context.SaveChanges();

            return View(viewModel);
        }
    }
}
