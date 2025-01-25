﻿
using BeestjeOpEenFeestje.Models;
using BeestjeOpEenFeestje.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
        public async Task<IActionResult> CreateAnimal(CreateAnimalModel model)
        {
            if(MakeAnimal(model))
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
                return true;
            }
            return false;
        }

        public IActionResult AnimalList()
        {
            List<AnimalInfoModel> list = new();
            foreach (Animal animal in _context.Animals.Include(e => e.Reservations.Where(e => e.IsConfirmed == true)))
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

                    reservations.Add(new ReservationInfoModel() { Date = reservationDate, Username = userName });
                }
                list.Add(new AnimalInfoModel() { Id = animal.Id, Name = animal.Name, ImageURL = animal.ImageURL, Price = animal.Price, Type = animal.Type });
            }
            return View(list);
        }

        //public async Task<IActionResult> UpdateUser(string email)
        //{
        //    if (string.IsNullOrEmpty(email)) return RedirectToAction("AccountsList");

        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user != null)
        //    {
        //        UpdateUserModel model = new() { Address = user.Address, Email = email, PhoneNumber = user.PhoneNumber, Username = user.UserName, CustomerCard = user.CustomerCard };
        //        return View(model);
        //    }
        //    return RedirectToAction("AccountsList");
        //}

        //[HttpPost]
        //public async Task<IActionResult> UpdateUser(UpdateUserModel model)
        //{
        //    if (await ChangeUser(model))
        //    {
        //        return RedirectToAction("AccountsList");
        //    }
        //    return View(model);
        //}

        //private async Task<bool> ChangeUser(UpdateUserModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var existingUser = await _userManager.FindByNameAsync(model.Username);
        //        var user = await _userManager.FindByEmailAsync(model.Email);

        //        if (user != null)
        //        {
        //            if (existingUser != null && existingUser != user)
        //            {
        //                ModelState.AddModelError("Username", "De gebruikersnaam is al in gebruik. Kies een andere gebruikersnaam.");
        //                return false;
        //            }
        //            user.Address = model.Address;
        //            user.PhoneNumber = model.PhoneNumber;
        //            user.UserName = model.Username;
        //            user.CustomerCard = model.CustomerCard;
        //            var result = await _userManager.UpdateAsync(user);
        //            if (result.Succeeded) return true;
        //        }
        //    }
        //    return false;
        //}

        private List<string> GetImageUrls()
        {
            string baseDirectory = Directory.GetCurrentDirectory();

            string projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, "../"));

            string imagesFolder = Path.Combine(projectRoot, "wwwroot", "Images");

            if (!Directory.Exists(imagesFolder))
            {
                return new List<string>();
            }

            string[] imageFiles = Directory.GetFiles(imagesFolder, "*.png", SearchOption.TopDirectoryOnly);

            List<string> imageUrls = new List<string>();
            foreach (string imageFile in imageFiles)
            {
                string relativePath = imageFile.Substring(imageFile.IndexOf("wwwroot") + "wwwroot".Length).Replace("\\", "/");
                imageUrls.Add(relativePath);
            }

            return imageUrls;
        }
    }
}
