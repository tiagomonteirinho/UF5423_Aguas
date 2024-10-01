using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Models;

namespace UF5423_Aguas.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> RegisterUserAsync(User user, string password);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<IdentityResult> EditUserAsync(User user);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task EnsureCreatedRoleAsync(string role);

        Task AddUserToRoleAsync(User user, string role);

        Task<bool> IsUserInRoleAsync(User user, string role);
    }
}
