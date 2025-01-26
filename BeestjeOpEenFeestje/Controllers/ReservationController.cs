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
            if (viewModel.SelectedAnimals != null)
            {
                foreach (int animalId in viewModel.SelectedAnimals)
                {
                    Animal a = _context.Animals.Where(a => a.Id == animalId).FirstOrDefault()
                        ?? throw new Exception("Beestje bestaat niet.");
                    animals.Add(a);
                }
            }

            else if (viewModel.SelectedAnimals == null || !await AreAnimalsAllowed(animals, reservation.Date))
            {
                ReservationModel rm = new ReservationModel
                {
                    Reservation = reservation,
                    Animals = GetAvailableAnimals(reservation.Date),
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
                return RedirectToAction("AutoPost", r);
            }

            return View(r);
        }

        [HttpGet]
        public IActionResult AutoPost(Reservation reservation)
        {
            return View(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCustomerInfo(Reservation reservation)
        {
            Reservation r = GetReservation(reservation.Id);

            if (_signInManager.IsSignedIn(User))
            {
                AppUser user = await _signInManager.UserManager.GetUserAsync(User);
                if (user != null)
                {
                    r.AppUser = user;
                    r.Name = user.UserName;
                    r.Email = user.Email;
                    r.Address = user.Address;
                }
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
        public async Task<IActionResult> Step3(Reservation reservation)
        {
            Reservation r = GetReservation(reservation.Id);

            double priceWithoutDiscount = r.Animals.Select(a => a.Price).Sum();
            double price = ApplyDiscount(priceWithoutDiscount, await CalculateDiscount(r.Animals, r.Date));

            ReservationPriceModel viewModel = new ReservationPriceModel
            {
                Reservation = r,
                PriceTotal = price,
                
            };

            return View(r);
        }

        [HttpPost]
        public IActionResult Confirm(Reservation reservation)
        {
            Reservation r = GetReservation(reservation.Id);

            // Check if animals are still available
            List<Animal> animals = GetAvailableAnimals(r.Date);
            foreach (Animal ra in r.Animals)
            {
                if (!animals.Where(a => a.Id == ra.Id).Any())
                {
                    r.Animals.Clear();
                    _context.SaveChanges();

                    ReservationModel viewModel = new ReservationModel
                    {
                        Reservation = r,
                        Animals = animals,
                    };

                    ModelState.Clear();
                    ModelState.AddModelError("available", "Eerder geselecteerde dier(en) zijn niet meer beschikbaar.");
                    return View("Step1", viewModel);
                }
            }

            r.IsConfirmed = true;
            _context.SaveChanges();

            return View();
        }

        private double ApplyDiscount(double price, List<Discount> discounts)
        {
            foreach (int discount in discounts.Select(d => d.Amount).ToList())
            {
                price *= (discount / 100);
            }
            return price;
        }

        private async Task<List<Discount>> CalculateDiscount(List<Animal> animals, DateOnly date)
        {
            List<Discount> discounts = new List<Discount>();
            AppUser? user = await _signInManager.UserManager.GetUserAsync(User);
            
            if (date.DayOfWeek.Equals(DayOfWeek.Monday) || date.DayOfWeek.Equals(DayOfWeek.Tuesday))
            {
                discounts.Add(new Discount(DiscountNames.MonOrTue, MonTueDiscount));
            }
            if (user != null && !user.CustomerCard.Equals(CustomerCardType.Geen))
            {
                discounts.Add(new Discount(DiscountNames.CustomerCard, CardDiscount));
            }
            if (animals.GroupBy(a => a.Type).Any(group => group.Count() >= 3))
            {
                discounts.Add(new Discount(DiscountNames.ThreeTypes, ThreeSameType));
            }
            if (animals.Where(a => a.Name.ToLower().Equals("eend")).Any())
            {
                Random random = new Random();
                int result = random.Next(1, 7);
                if (result == 6)
                {
                    discounts.Add(new Discount(DiscountNames.Duck, LuckyDuck));
                }
            }
            foreach (Animal animal in animals)
            {

            }

            while (discounts.Select(d => d.Amount).Sum() > 60)
            {
                discounts.Remove(discounts.Where(d => d.Amount == discounts.Select(d => d.Amount).Min()).FirstOrDefault());
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
            List<Reservation> reservationsOnDate = _context.Reservations
                .Where(r => r.Date.Equals(date))
                .Where(r => r.IsConfirmed.Equals(true))
                .Include(r => r.Animals)
                .ToList();

            foreach (Animal animal in _context.Animals.ToList())
            {
                bool isAvailable = true;
                foreach (Reservation reservation in reservationsOnDate)
                {
                    if (reservation.Animals.Where(a => a.Id == animal.Id).Any())
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
