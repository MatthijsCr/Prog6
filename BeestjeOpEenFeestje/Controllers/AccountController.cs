﻿
using BeestjeOpEenFeestje.Models;
using BeestjeOpEenFeestje.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;

namespace BumboApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: false);
                    await _signInManager.RefreshSignInAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index","Home");
                    }
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin,Employee")]
        public IActionResult CreateUser()
        {
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> CreateUser(CreateUserModel model)
        {
            ViewBag.Roles = _roleManager.Roles.ToList();
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByNameAsync(model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "De gebruikersnaam is al in gebruik. Kies een andere gebruikersnaam.");
                    return View(model);
                }
                var roleExists = await _roleManager.RoleExistsAsync(model.Role.ToString());
                if (!roleExists)
                {
                    return View(model);
                }

                var user = new AppUser
                {
                    UserName = model.Username,
                    CustomerCard = model.CustomerCard.ToString(),
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address
                };
                string GeneratedPassword = GeneratePassword(10);
                PasswordHasher<AppUser> passwordHasher = new();
                user.PasswordHash = passwordHasher.HashPassword(user, GeneratedPassword);
                
                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    return RedirectToAction("Password", new { password = GeneratedPassword, email = model.Email });
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Password(string password, string Email) => View(model: new LoginModel() { Email = Email, Password = password});

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public string GeneratePassword(int length)
        {
            if(length <= 0) return string.Empty;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@#!";
            Random random = new Random();
            string password = string.Empty;

            for (int i = 0; i <= length; i++)
            {
                password += chars[random.Next(chars.Length)];
            }

            return password;
        }

        public IActionResult AccountsList()
        {
            List<AccountListModel> list = new();
            foreach(AppUser user in _userManager.Users)
            {
                list.Add(new AccountListModel() 
                { 
                    Address = user.Address, 
                    CustomerCard = user.CustomerCard, 
                    Email = user.Email, 
                    PhoneNumber = user.PhoneNumber, 
                    Username = user.UserName 
                });
            }
            return View(list);
        }

        public async Task<IActionResult> UpdateUser(string email)
        {
            if (string.IsNullOrEmpty(email)) return RedirectToAction("AccountsList");

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                UpdateUserModel model = new() { Address = user.Address, Email = email, PhoneNumber = user.PhoneNumber, Username = user.UserName, CustomerCard = (CustomerCardType?)Enum.Parse(typeof(CustomerCardType), user.CustomerCard) };
                return View(model);
            }
            return RedirectToAction("AccountsList");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UpdateUserModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByNameAsync(model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "De gebruikersnaam is al in gebruik. Kies een andere gebruikersnaam.");
                    return View(model);
                }
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;
                    user.UserName = model.Username;
                    user.CustomerCard = model.CustomerCard.ToString();
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded) return RedirectToAction("AccountsList");
                }
            }
            return View(model);
        }
    }
}
