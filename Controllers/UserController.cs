using Asp_MVC.Dto;
using Asp_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Asp_MVC.Controllers
{
    [Authorize]
    public class UserController(UserManager<User> userManager, SignInManager<User> signInManager,
                                    RoleManager<IdentityRole<long>> roleManager) : Controller
{
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = new UserViewModel();
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            viewModel.CurrentUser = ConvertToUserDto(currentUser);

            if (await userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                var usersInUserRole = new List<UserDto>();
                var allUsers = userManager.Users.ToList();

                foreach (var user in allUsers)
                {
                    if (await userManager.IsInRoleAsync(user, "User"))
                    {
                        usersInUserRole.Add(ConvertToUserDto(user));
                    }
                }

                viewModel.Users = usersInUserRole.OrderBy(u => u.Name).ToList();
            }

            return View("User_Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUser(long id)
        {
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var userToEdit = await userManager.FindByIdAsync(id.ToString());
            if (userToEdit == null)
            {
                return NotFound("Użytkownik nie istnieje.");
            }

            if (!userToEdit.Email.Equals(currentUser.Email) &&
                !await userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                return Forbid("Nie masz dostępu do edycji tego użytkownika.");
            }

            var userEditDto = new UserRegistrationDto
            {
                Id = userToEdit.Id,
                Name = userToEdit.Name,
                PhoneNumber = userToEdit.PhoneNumber,
                Email = userToEdit.Email,
                Password = ""
            };

            return View("User_Update",userEditDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(long id, UserRegistrationDto model)
        {
            if (!ModelState.IsValid)
            {
                model.Id = id;
                return View("User_Update", model);
            }

            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var userToEdit = await userManager.FindByIdAsync(id.ToString());
            if (userToEdit == null)
            {
                return Forbid("Użytkownik nie istnieje.");
            }

            if (!userToEdit.Email.Equals(currentUser.Email) &&
                !await userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                return Forbid("Nie masz dostępu do edycji tego użytkownika.");
            }

            userToEdit.Name = model.Name;
            userToEdit.PhoneNumber = model.PhoneNumber;
            userToEdit.PasswordHash = new PasswordHasher<User>().HashPassword(null, model.Password);

            if (userToEdit.Email != model.Email)
            {
                var setEmailResult = await userManager.SetEmailAsync(userToEdit, model.Email);
                var setUsernameResult = await userManager.SetUserNameAsync(userToEdit, model.Email);
                if (!setEmailResult.Succeeded || !setUsernameResult.Succeeded)
                {
                    TempData["EmailInvalid"] = true;
                    model.Id = id;
                    return View("User_Update", model);
                }
            }

            await userManager.UpdateAsync(userToEdit);

            if (userToEdit.Email.Equals(currentUser.Email))
                return RedirectToAction("Logout", "Account");
            TempData["Success"] = true;
            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var userToDelete = await userManager.FindByIdAsync(id.ToString());
            if (userToDelete == null)
            {
                return NotFound("Użytkownik nie istnieje.");
            }

            if (userToDelete.Email.Equals(currentUser.Email) ||
                !await userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                return Forbid("Nie masz dostępu do usunięcia tego użytkownika.");
            }

            var result = await userManager.DeleteAsync(userToDelete);

            return RedirectToAction("Index", "User");
        }

        private UserDto ConvertToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }
    }
}
