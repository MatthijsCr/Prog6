using BeestjeOpEenFeestje.Models;
using BeestjeOpEenFeestje.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Security.Principal;

namespace BeestjeOpEenFeestje.Controllers
{
    public class ReservationController(AnimalDbContext context, SignInManager<AppUser> signInManager) : Controller
    {
        private const int MaxDiscount = 60;
        private const int MonTueDiscount = 15;
        private const int LuckyDuck = 50;
        private const int ThreeSameType = 10;
        private const int CardDiscount = 10;
        private const int LetterCombo = 2;


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
        public async Task<IActionResult> SaveAnimals(ReservationModel viewModel)
        {
            Reservation reservation = GetReservation(viewModel.Reservation.Id);

            List<Animal> animals = new List<Animal>();
            foreach (string animal in viewModel.SelectedAnimals)
            {
                Animal a = _context.Animals.Where(a => a.Name.ToLower().Equals(animal.ToLower())).FirstOrDefault()
                    ?? throw new Exception("Beestje " + animal + " bestaat niet.");
                animals.Add(a);
            }

            if (!ModelState.IsValid || !await AreAnimalsAllowed(animals, reservation.Date))
            {
                ReservationModel rm = new()
                {
                    Reservation = reservation,
                    Animals = GetAvailableAnimals(reservation.Date),
                    SelectedAnimals = new(),
                };
                return View("Step1", rm);
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

        private async Task<List<int>> CalculateDiscount(List<Animal> animals, DateOnly date)
        {
            List<int> discounts = new List<int>();
            AppUser? user = await _signInManager.UserManager.GetUserAsync(User);
            
            if (date.DayOfWeek.Equals(DayOfWeek.Monday) || date.DayOfWeek.Equals(DayOfWeek.Tuesday))
            {
                discounts.Add(MonTueDiscount);
            }
            if (user != null && !user.CustomerCard.Equals(CustomerCardType.Geen))
            {
                discounts.Add(CardDiscount);
            }


            while (discounts.Sum() > 60)
            {
                discounts.Remove(discounts.Min());
            }

            return discounts;
        }

        private async Task<bool> AreAnimalsAllowed(List<Animal> animals, DateOnly date)
        {
            AppUser? user = await _signInManager.UserManager.GetUserAsync(User);

            if (animals.Where(a => a.Type.ToLower().Equals("boerderij")).Any())
            {
                if (animals.Where(a => a.Name.ToLower().Equals("leeuw")).Any())
                    return false;
                else if (animals.Where(a => a.Name.ToLower().Equals("ijsbeer")).Any())
                    return false;
            }

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                if (animals.Where(a => a.Name.ToLower().Equals("pinguïn")).Any())
                    return false;
            }

            if (date.Month >= 10 || date.Month <= 2)
            {
                if (animals.Where(a => a.Type.ToLower().Equals("woestijn")).Any())
                    return false;
            }

            if (date.Month >= 6 && date.Month <= 8)
            {
                if (animals.Where(a => a.Type.ToLower().Equals("sneeuw")).Any())
                    return false;
            }

            if (user != null)
            {
                if (user.CustomerCard.Equals(CustomerCardType.Geen) && animals.Count > 3)
                {
                    if (!date.DayOfWeek.Equals(DayOfWeek.Wednesday) && animals.Count <= 4) // Self added rule for signed in users without customer card
                        return false;
                }
                else if (user.CustomerCard.Equals(CustomerCardType.Zilver) && animals.Count > 4)
                    return false;
                
                if (!user.CustomerCard.Equals(CustomerCardType.Platinum) && animals.Where(a => a.Type.ToLower().Equals("vip")).Any())
                {
                    return false;
                }
            }
            else if (animals.Count > 3 || animals.Where(a => a.Type.ToLower().Equals("vip")).Any())
            {
                return false;
            }

            return true;
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
