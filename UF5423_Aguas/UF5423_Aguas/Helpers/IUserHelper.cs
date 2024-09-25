using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Models;

namespace UF5423_Aguas.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
    }
}
