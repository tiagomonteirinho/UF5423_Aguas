using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using Water_Services.Data.Entities;
using Water_Services.Models;

namespace Water_Services.Helpers
{
    public interface IUserHelper
    {
        Task<List<User>> GetAllUsersAsync();

        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserByIdAsync(string id);

        Task<IdentityResult> RegisterUserAsync(User user, string password);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<IdentityResult> ChangeInfoAsync(User user);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task EnsureCreatedRoleAsync(string role);

        Task AddUserToRoleAsync(User user, string role);

        Task<bool> IsUserInRoleAsync(User user, string role);

        Task<List<IdentityRole>> GetAllRolesAsync();

        Task<IList<string>> GetUserRolesAsync(User user);

        IEnumerable<SelectListItem> GetComboRoles();

        Task<IEnumerable<SelectListItem>> GetComboUsers();

        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmAccountAsync(User user, string token);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> SetPasswordAsync(User user, string token, string password);
    }
}
