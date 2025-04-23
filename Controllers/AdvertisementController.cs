using System.Reflection;
using Asp_MVC.Data;
using Asp_MVC.Dto;
using Asp_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asp_MVC.Controllers
{
    [Authorize]
    public class AdvertisementController(AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager,
                                    RoleManager<IdentityRole<long>> roleManager) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(string title)
        {
            IQueryable<Advertisement> query = context.Advertisements;

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(l => EF.Functions.Like(l.Title.ToLower(), $"%{title.ToLower()}%"));
            }

            var advertisements = await query.OrderBy(l => l.Title).ToListAsync();


            return View("Advertisement_Index", advertisements);
        }

        [HttpGet]
        public IActionResult AddAdvertisement()
        {
            return View("Advertisement_Add");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdvertisement(AdvertisementCreationDto model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await userManager.FindByEmailAsync(User.Identity.Name);
                if (existingUser == null)
                {
                    return Unauthorized();
                }
                var date = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                var advertisement = new Advertisement
                {
                    UserId = existingUser.Id,
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price,
                    Duration = model.Duration,
                    CreatedAt = date
                };

                context.Advertisements.Add(advertisement);
                context.SaveChanges();

                return RedirectToAction("Index", "Advertisement");

            }
            return View("Advertisement_Add", model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateAdvertisement(int id)
        {

            var advertisement = context.Advertisements.FirstOrDefault(l => l.Id == id);
            if (advertisement == null)
            {
                return NotFound();
            }
            if (!await Check_If_Allowed(id,advertisement.UserId))
                return Forbid();
            var dto = new AdvertisementCreationDto(advertisement);
            dto.Id = id;
            return View("Advertisement_Update", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAdvertisement(AdvertisementCreationDto model, int id)
        {
 
            var advertisement = context.Advertisements.FirstOrDefault(l => l.Id == id);
            if (advertisement == null)
            {
                return NotFound();
            }
            if (!await Check_If_Allowed(id, advertisement.UserId))
                return Forbid();
            if (ModelState.IsValid)
            {
                advertisement.Title = model.Title;
                    advertisement.Description = model.Description;
                    advertisement.Price = model.Price;
                    advertisement.Duration = model.Duration;

                    context.Update(advertisement);
                    await context.SaveChangesAsync();

                    return RedirectToAction("Index", "Advertisement");
            }
            model.Id = id;
            return View("Advertisement_Update", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAdvertisement(int id)
        {
            var advertisement = context.Advertisements.FirstOrDefault(l => l.Id == id);
            if (advertisement == null)
            {
                return NotFound();
            }
            if (!await Check_If_Allowed(id, advertisement.UserId))
                return Forbid();
            context.Advertisements.Remove(advertisement);
            context.SaveChanges();
            return RedirectToAction("Index", "Advertisement");
        }

        private async Task<bool> Check_If_Allowed(int id, long UserId)
        {
            
            var existingUser = await userManager.FindByEmailAsync(User.Identity.Name);
            if (existingUser == null)
            {
                return false;
            }

            bool isAdmin = await userManager.IsInRoleAsync(existingUser, "Admin");

            if (UserId != existingUser.Id && !isAdmin)
            {
                return false;
            }
            return true;
        }

    }

}
