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
            // Get Reservation with auto-increment Id
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
                Reservation = reservation,
                Animals = availableAnimals,
            };

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

            ReservationModel rm = new ReservationModel
            {
                Reservation = reservation,
                Animals = GetAvailableAnimals(reservation.Date),
            };

            if (viewModel.SelectedAnimals.Count() == 0)
            {
                ModelState.Clear();
                ModelState.AddModelError("SelectedAnimals", "Selecteer minstens één beestje.");
                return View("Step1", rm);
            }
            else if (!await AreAnimalsAllowed(animals, reservation.Date))
            {
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

            List<Discount> discounts = await CalculateDiscount(r.Animals, r.Date);
            double priceWithoutDiscount = r.Animals.Select(a => a.Price).Sum();
            double price = ApplyDiscount(priceWithoutDiscount, discounts);

            ReservationPriceModel viewModel = new ReservationPriceModel
            {
                Reservation = r,
                PriceTotal = price,
                Discounts = discounts,
            };

            return View(viewModel);
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
                price *= ((100 - discount) / 100.0);
            }
            return price;
        }

        internal async Task<List<Discount>> CalculateDiscount(List<Animal> animals, DateOnly date)
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
                int comboAmount = 0;
                char letter = 'a';
                while (letter >= 'a' && letter <= 'z')
                {
                    if (animal.Name.ToLower().Contains(letter))
                    {
                        comboAmount++;
                        letter++;
                        continue;
                    }
                    break;
                }
                if (comboAmount > 0)
                {
                    discounts.Add(new Discount(DiscountNames.ABC, (LetterCombo * comboAmount)));
                    break;
                }  
            }

            while (discounts.Select(d => d.Amount).Sum() > MaxDiscount)
            {
                discounts.Remove(discounts.Where(d => d.Amount == discounts.Select(d => d.Amount).Min()).FirstOrDefault());
            }

            return discounts;
        }

        internal async Task<bool> AreAnimalsAllowed(List<Animal> animals, DateOnly date)
        {
            ModelState.Clear();
            AppUser? user = await _signInManager.UserManager.GetUserAsync(User);

            if (animals.Where(a => a.Type.ToLower().Equals("boerderij")).Any())
            {
                if (animals.Where(a => a.Name.ToLower().Equals("leeuw")).Any())
                {
                    ModelState.AddModelError("", "Mag geen beestje van type boerderij combineren met een leeuw.");
                    return false;
                }
                else if (animals.Where(a => a.Name.ToLower().Equals("ijsbeer")).Any())
                {
                    ModelState.AddModelError("", "Mag geen beestje van type boerderij combineren met een ijsbeer.");
                    return false;
                }
            }

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                if (animals.Where(a => a.Name.ToLower().Equals("pinguïn")).Any())
                {
                    ModelState.AddModelError("", "Dieren in pak werken alleen doordeweeks");
                    return false;
                if (animals.Where(a => a.Name.ToLower().Equals("pinguin")).Any())
                    return false;
            }

            if (date.Month >= 10 || date.Month <= 2)
            {
                if (animals.Where(a => a.Type.ToLower().Equals("woestijn")).Any())
                {
                    ModelState.AddModelError("", "Brrrr – Veelste koud");
                    return false;
                }
            }

            if (date.Month >= 6 && date.Month <= 8)
            {
                if (animals.Where(a => a.Type.ToLower().Equals("sneeuw")).Any())
                {
                    ModelState.AddModelError("", "Beestjes van type sneeuw mogen alleen tussen september en mei.");
                    return false;
                }
            }

            if (user != null)
            {
                if (user.CustomerCard.Equals(CustomerCardType.Geen) && animals.Count > 3)
                {
                    if (!date.DayOfWeek.Equals(DayOfWeek.Wednesday)) 
                    {
                        ModelState.AddModelError("", "Max. 3 beestjes zonder klantenkaart.");
                        return false;
                    }
                    else if (animals.Count > 4) // Self added rule for signed in users without customer card
                    {
                        ModelState.AddModelError("", "Max. 4 beestjes op woensdag zonder klantenkaart.");
                        return false;
                    }
                }
                else if (user.CustomerCard.Equals(CustomerCardType.Zilver) && animals.Count > 4)
                {
                    ModelState.AddModelError("", "Max. 4 beestjes met zilveren klantenkaart.");
                    return false;
                }
                if (!user.CustomerCard.Equals(CustomerCardType.Platinum) && animals.Where(a => a.Type.ToLower().Equals("vip")).Any())
                {
                    ModelState.AddModelError("", "VIP beestjes alleen met platinum klantenkaart.");
                    return false;
                }
            }
            else if (animals.Count > 3)
            {
                ModelState.AddModelError("", "Max. 3 beestjes zonder klantenkaart.");
                return false;
            }
            else if (animals.Where(a => a.Type.ToLower().Equals("vip")).Any())
            {
                ModelState.AddModelError("", "VIP beestjes alleen met platinum klantenkaart.");
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
