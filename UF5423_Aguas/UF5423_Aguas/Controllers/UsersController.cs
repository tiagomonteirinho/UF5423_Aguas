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
            //TODO: add to generic repository to have GetAll().OrderBy().
            var users = await _userRepository.GetAllUsersAsync();
            return View(users);
        }

        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    var path = string.Empty;
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var guid = Guid.NewGuid().ToString();
                        string fileName = $"{guid}.jpg";

                        path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot\\images\\users",
                            fileName
                        );

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(stream);
                        }

                        path = $"~/images/users/{fileName}";
                    }
                    else
                    {
                        path = "~/images/users/default.png";
                    }

                    user = this.ConvertToUser(model, path);

                    var result = await _userHelper.RegisterUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "Could not register user account.");
                    }

                    await _userHelper.AddUserToRoleAsync(user, "Customer");
                    var isInRole = await _userHelper.IsUserInRoleAsync(user, "Customer");
                    if (!isInRole)
                    {
                        ModelState.AddModelError(string.Empty, "Could not register user account.");
                    }

                    //TODO: Replace by ViewBag;
                    ModelState.AddModelError(string.Empty, "User successfully registered.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "A user already exists with that email address.");
                }
            }

            return View(model);
        }

        private User ConvertToUser(RegisterViewModel model, string path)
        {
            return new User
            {
                Email = model.Email,
                UserName = model.Email,
                FullName = model.FullName,
                ImageUrl = path,
            };
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = this.ConvertToUserViewModel(user);
            return View(viewModel);
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

        private EditUserViewModel ConvertToUserViewModel(User user)
        {
            return new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
            };
        }
    }
}
