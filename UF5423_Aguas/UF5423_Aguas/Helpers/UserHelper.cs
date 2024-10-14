using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Models;

namespace UF5423_Aguas.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserHelper(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var list = await _userManager.Users.OrderBy(u => u.FullName).ToListAsync();
            return list;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> RegisterUserAsync(User user, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return await _userManager.CreateAsync(user);
            }
            else
            {
                return await _userManager.CreateAsync(user, password);
            }
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.StaySignedIn, false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ChangeInfoAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task EnsureCreatedRoleAsync(string role)
        {
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = role });
            }
        }

        public async Task AddUserToRoleAsync(User user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);
            user.RoleName = role;
        }

        public async Task<bool> IsUserInRoleAsync(User user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            var list = await _userManager.GetRolesAsync(user);
            return list;
        }

        public IEnumerable<SelectListItem> GetComboRoles()
        {
            var list = _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
            }).OrderBy(i => i.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a role...)",
                Value = string.Empty,
            });

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboUsers()
        {
            var customers = new List<User>();
            foreach (User user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, "Customer"))
                {
                    customers.Add(user);
                }
            }

            var list = customers
                .AsEnumerable() // Allow custom Text string.
                .Select(u => new SelectListItem
                {
                    Text = $"{u.FullName} ({u.Email})",
                    Value = u.Email,
                }).OrderBy(i => i.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a user...)",
                Value = string.Empty,
            });

            return list;
        }

        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, false);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmAccountAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> SetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }
    }
}
