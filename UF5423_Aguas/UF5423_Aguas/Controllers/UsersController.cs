using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using UF5423_Aguas.Data;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Helpers;
using UF5423_Aguas.Models;

namespace UF5423_Aguas.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserHelper _userHelper;

        public UsersController(IUserHelper userHelper, IUserRepository userRepository)
        {
            _userHelper = userHelper;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllAsync(); 
            foreach (var user in users)
            {
                user.RoleName = (await _userHelper.GetUserRolesAsync(user)).FirstOrDefault();
            }

            return View(users);
        }

        public IActionResult Create()
        {
            return View(new UserViewModel
            {
                Roles = _userHelper.GetComboRoles()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            model.Roles = _userHelper.GetComboRoles(); // Repopulate roles.
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user != null)
                {
                    ViewBag.ErrorMessage = "That email is already being used.";
                    return View(model);
                }
                else
                {
                    user = new User
                    {
                        Email = model.Email,
                        UserName = model.Email,
                        FullName = model.FullName,
                        ImageUrl = $"~/images/default_profile_picture.jpg",
                    };

                    var result = await _userHelper.RegisterUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        ViewBag.ErrorMessage = "Could not create user.";
                        return View(model);
                    }

                    await _userHelper.AddUserToRoleAsync(user, model.RoleName);
                    var isInRole = await _userHelper.IsUserInRoleAsync(user, model.RoleName);
                    if (!isInRole)
                    {
                        ViewBag.ErrorMessage = "Could not add user to role.";
                        return View(model);
                    }

                    ViewBag.SuccessMessage = "User created successfully!";

                    ModelState.Clear(); // Clear form.
                    return View(new UserViewModel
                    {
                        Roles = _userHelper.GetComboRoles() // Keep roles.
                    });
                }
            }

            ViewBag.ErrorMessage = "Could not create user.";
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            try
            {
                await _userRepository.DeleteByIdAsync(id);
                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    return RedirectToAction("Error", "Errors", new
                    {
                        title = $"User deletion error.",
                        message = $"The user {user.FullName} could not be deleted. Please ensure that they are not being used by other entities.",
                    });
                }

                return RedirectToAction("Error", "Errors");
            }
        }
    }
}
