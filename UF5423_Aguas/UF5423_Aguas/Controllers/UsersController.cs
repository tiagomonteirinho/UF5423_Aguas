using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
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
        private readonly IUserHelper _userHelper;
        private readonly IUserRepository _userRepository;

        public UsersController(IUserHelper userHelper, IUserRepository userRepository)
        {
            _userHelper = userHelper;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return View(users);
        }

        public IActionResult Create()
        {
            var model = new UserViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
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
                    }

                    await _userHelper.AddUserToRoleAsync(user, "Customer");
                    var isInRole = await _userHelper.IsUserInRoleAsync(user, "Customer");
                    if (!isInRole)
                    {
                        ViewBag.ErrorMessage = "Could not add user to role.";
                    }

                    ViewBag.SuccessMessage = "User created successfully.";
                    ModelState.Clear(); // Clear form.
                    return View();
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetUserByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Email = model.Email;
                user.UserName = model.Email;
                user.FullName = model.FullName;

                var result = await _userHelper.EditUserAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }
    }
}
