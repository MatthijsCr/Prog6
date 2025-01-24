using BeestjeOpEenFeestje.Models;
using BeestjeOpEenFeestje.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BeestjeOpEenFeestje.Controllers
{
    public class ReservationController(AnimalDbContext context, SignInManager<AppUser> signInManager) : Controller
    {
        private readonly AnimalDbContext _context = context;
        private readonly SignInManager<AppUser> _signInManager = signInManager;

        [HttpGet]
        public IActionResult Date()
        {
            return View();
        }

        [HttpPost]
        [Route("Reserveren/Beestjes")]
        public IActionResult Step1(DateOnly date)
        {
            EntityEntry<Reservation> entry = _context.Add(new Reservation() { Date = date });
            _context.SaveChanges();

            List<Animal> availableAnimals = GetAvailableAnimals(date);

            if (!availableAnimals.Any())
            {
                ModelState.AddModelError("date", "Er zijn helaas geen beestjes beschikbaar op deze datum.");
            }

            ReservationModel viewModel = new ReservationModel
            {
                Reservation = entry.Entity, // Reservation with Id set in db
                Animals = availableAnimals,
                SelectedAnimals = new(),
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Step2(ReservationModel viewModel)
        {
            Reservation reservation = _context.Reservations
                .Where(r => r.Id == viewModel.Reservation.Id)
                .Include(r => r.Animals)
                .FirstOrDefault()
                ?? throw new Exception("Kan reservering niet ophalen.");

            if (viewModel.SelectedAnimals == null)
            {
                ModelState.AddModelError("animal", "Selecteer minstens één beestje.");

                ReservationModel reservationModel = new()
                {
                    Reservation = reservation,
                    Animals = GetAvailableAnimals(reservation.Date),
                    SelectedAnimals = new(),
                };

                return View("Step1", reservationModel);
            }

            List<Animal> animals = new List<Animal>();
            foreach (string animal in viewModel.SelectedAnimals)
            {
                Animal a = _context.Animals.Where(a => a.Name.ToLower().Equals(animal.ToLower())).FirstOrDefault() 
                    ?? throw new Exception("Beestje " + animal + " bestaat niet.");
                animals.Add(a);
            }

            reservation.Animals.AddRange(animals);
            _context.SaveChanges();

            if (_signInManager.IsSignedIn(User))
            {
                RedirectToAction("Step3", reservation);
            }

            viewModel.Reservation = reservation;
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Step3(ReservationModel viewModel)
        {
            Reservation r = _context.Reservations
                .Where(r => r.Id == viewModel.Reservation.Id)
                .Include(r => r.Animals)
                .FirstOrDefault()
                ?? throw new Exception("Kan reservering niet vinden.");

            return View(r);
        }

        [HttpPost]
        public IActionResult Confirm()
        {
            return View();
        }


        private List<Animal> GetAvailableAnimals(DateOnly date)
        {
            List<Animal> availableAnimals = new List<Animal>();
            List<Reservation> reservationsOnDate = _context.Reservations.Where(r => r.Date.Equals(date)).ToList();

            foreach (Animal animal in _context.Animals.ToList())
            {
                bool isAvailable = true;
                foreach (Reservation reservation in reservationsOnDate)
                {
                    if (reservation.Animals.Contains(animal))
                    {
                        isAvailable = false;
                        break;
                    }
                }
                if (isAvailable)
                {
                    availableAnimals.Add(animal);
                }
            }

            return availableAnimals;
        }
    }
}
