using System;
using System.Diagnostics;
using Asp_MVC.Dto;
using Asp_MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Asp_MVC.Controllers
{
    public class AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
                                    RoleManager<IdentityRole<long>> roleManager) : Controller
    {
       

        [HttpGet]
        public IActionResult Register()
        {
 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegistrationDto model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    TempData["EmailInvalid"] = true;
                    return View(model);
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, model.Password)
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!await roleManager.RoleExistsAsync("User"))
                    {
                        await roleManager.CreateAsync(new IdentityRole<long>("User"));
                    }

                    await userManager.AddToRoleAsync(user, "User");


                    TempData["Success"] = "Rejestracja ukończona pomyślnie";
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await userManager.FindByEmailAsync(model.Email);
                if (existingUser == null)
                {
                    TempData["Error"] = "Niepoprawne dane logowania.";
                    return View(model);
                }
                var loginSuccessful = await signInManager.CheckPasswordSignInAsync
                    (existingUser, model.Password, false);
                if (loginSuccessful.Succeeded)
                {
                    await signInManager.SignInAsync(existingUser, false);
                    return RedirectToAction("Index", "Advertisement");
                }
                else
                {
                    TempData["Error"] = "Niepoprawne dane logowania.";
                    return View();
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            TempData["Logout"] = "Zostałeś wylogowany";
            return RedirectToAction("Index", "Home");
        }
    }
}
