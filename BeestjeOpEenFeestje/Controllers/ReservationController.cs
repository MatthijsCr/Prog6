using BeestjeOpEenFeestje.Models;
using BeestjeOpEenFeestje.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.Contracts;

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
        public IActionResult SaveDate(DateOnly date)
        {
            EntityEntry<Reservation> entry = _context.Add(new Reservation() { Date = date });
            _context.SaveChanges();

            return RedirectToAction("Step1", entry.Entity);
        }

        [HttpGet]
        [Route("Reserveren/Beestjes")]
        public IActionResult Step1(Reservation reservation)
        {
            List<Animal> availableAnimals = GetAvailableAnimals(reservation.Date);

            if (!availableAnimals.Any())
            {
                ModelState.AddModelError("date", "Er zijn helaas geen beestjes beschikbaar op deze datum.");
            }

            ReservationModel viewModel = new ReservationModel
            {
                Reservation = reservation, // Reservation with Id set in db
                Animals = availableAnimals,
                SelectedAnimals = new(),
            };

            if (TempData["SelectedAnimalsError"] != null)
            {
                ModelState.AddModelError("animal", "Selecteer minstens één beestje.");
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SaveAnimals(ReservationModel viewModel)
        {
            Reservation reservation = GetReservation(viewModel.Reservation.Id);

            if (viewModel.SelectedAnimals == null)
            {
                TempData["SelectedAnimalsError"] = "true";
                return RedirectToAction("Step1", reservation);
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

            return RedirectToAction("Step2", reservation);
        }

        [HttpGet]
        public IActionResult Step2(Reservation reservation)
        {
            Reservation r = GetReservation(reservation.Id);

            if (_signInManager.IsSignedIn(User))
            {
                RedirectToAction("SaveCustomerInfo", r);
            }

            return View(r);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCustomerInfo(Reservation reservation)
        {
            Reservation r = GetReservation(reservation.Id);

            if (_signInManager.IsSignedIn(User))
            {
                r.AppUser = await _signInManager.UserManager.GetUserAsync(User);
            }
            else
            {
                r.Name = reservation.Name;
                r.Email = reservation.Email;
                r.Address = reservation.Address;
            }
            _context.SaveChanges();

            return RedirectToAction("Step3", r);
        }

        [HttpGet]
        public IActionResult Step3(Reservation reservation)
        {
            Reservation r = GetReservation(reservation.Id);
            return View(r);
        }

        [HttpPost]
        public IActionResult Confirm(Reservation reservation)
        {
            Reservation r = GetReservation(reservation.Id);

            r.IsConfirmed = true;
            _context.SaveChanges();

            return View();
        }

        private Reservation GetReservation(int id)
        {
            return _context.Reservations
                .Where(r => r.Id == id)
                .Include(r => r.Animals)
                .FirstOrDefault()
                ?? throw new Exception("Kan reservering niet ophalen.");
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
