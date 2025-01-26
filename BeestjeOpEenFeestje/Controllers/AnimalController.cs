
using BeestjeOpEenFeestje.Models;
using BeestjeOpEenFeestje.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace BeestjeOpEenFeestje.Controllers
{
    public class AnimalController : Controller
    {
        private readonly AnimalDbContext _context;

        public AnimalController(AnimalDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Employee")]
        public IActionResult CreateAnimal()
        {
            ViewBag.ImgUrls = GetImageUrls();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult CreateAnimal(CreateAnimalModel model)
        {
            if (MakeAnimal(model))
            {
                return RedirectToAction("AnimalList");
            }
            ViewBag.ImgUrls = GetImageUrls();
            return View(model);
        }

        private bool MakeAnimal(CreateAnimalModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Price < 0)
                {
                    ModelState.AddModelError("Price", "Prijs mag niet negatief zijn");
                    return false;
                }
                _context.Animals.Add(new Animal() { ImageURL = model.ImageURL, Name = model.Name, Price = model.Price, Reservations = new List<Reservation>(), Type = model.Type });
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public IActionResult AnimalList()
        {
            List<UpdateAnimalModel> list = new();
            foreach (Animal animal in _context.Animals.Include(e => e.Reservations.Where(e => e.IsConfirmed == true)))
            {
                list.Add(new UpdateAnimalModel() { Id = animal.Id, Name = animal.Name, ImageURL = animal.ImageURL, Price = animal.Price, Type = animal.Type });
            }
            return View(list);
        }

        public IActionResult Reservations(int Id)
        {
            Animal? animal = _context.Animals.Include(e => e.Reservations).FirstOrDefault(e => e.Id == Id);

            if (animal != null)
            {
                List<ReservationInfoModel> reservations = new();
                foreach (Reservation reservation in animal.Reservations)
                {
                    DateOnly reservationDate = reservation.Date;
                    string userName;
                    if (reservation.AppUser != null && reservation.AppUser.UserName != null)
                    {
                        userName = reservation.AppUser.UserName;
                    }
                    else if (reservation.Name != null)
                    {
                        userName = reservation.Name;
                    }
                    else continue;

                    reservations.Add(new ReservationInfoModel() { Date = reservationDate, Username = userName, AnimalName = animal.Name });
                }
                return View(reservations);
            }
            return RedirectToAction("AnimalList");
        }

        public IActionResult UpdateAnimal(int id)
        {
            Animal? animal = _context.Animals.FirstOrDefault(e => e.Id == id);

            if (animal != null)
            {
                UpdateAnimalModel model = new UpdateAnimalModel() { Id = id, Name = animal.Name, Price = animal.Price, ImageURL = animal.ImageURL, Type = animal.Type };
                ViewBag.ImgUrls = GetImageUrls();
                return View(model);
            }
            return RedirectToAction("AnimalList");
        }

        [HttpPost]
        public IActionResult UpdateAnimal(UpdateAnimalModel model)
        {
            if (ChangeAnimal(model))
            {
                return RedirectToAction("AnimalList");
            }
            ViewBag.ImgUrls = GetImageUrls();
            return View(model);
        }

        private bool ChangeAnimal(UpdateAnimalModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Price < 0)
                {
                    ModelState.AddModelError("Price", "Prijs mag niet negatief zijn");
                    return false;
                }
                Animal? animal = _context.Animals.FirstOrDefault(e => e.Id == model.Id);

                if (animal != null)
                {
                    animal.Price = model.Price;
                    animal.Name = model.Name;
                    animal.Type = model.Type;
                    animal.ImageURL = model.ImageURL;
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public IActionResult DeleteAnimal(int Id)
        {
            Animal? animal = _context.Animals.FirstOrDefault(e => e.Id == Id);

            if (animal != null)
            {
                _context.Animals.Remove(animal);
                _context.SaveChanges();
            }
            return RedirectToAction("AnimalList");
        }

        private List<string> GetImageUrls()
        {
            string imagesFolderPath = Path.Combine("wwwroot", "Images");

            if (!Directory.Exists(imagesFolderPath))
            {
                return new List<string>(); 
            }

            string[] imageFiles = Directory.GetFiles(imagesFolderPath, "*.png");

            List<string> imageUrls = new List<string>();
            foreach (var imageFile in imageFiles)
            {
                string relativePath = "/Images/" + Path.GetFileName(imageFile); 
                imageUrls.Add(relativePath);
            }

            ViewBag.ImageUrls = imageUrls;

            return imageUrls;
        }



    }
}