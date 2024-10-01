using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Helpers;

namespace UF5423_Aguas.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            var user = await _userHelper.GetUserByEmailAsync("admin.5423@yopmail.com");
            if (user == null)
            {
                user = new User
                {
                    FullName = "Admin",
                    Email = "admin.5423@yopmail.com",
                    UserName = "admin.5423@yopmail.com",
                };

                var result = await _userHelper.RegisterUserAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Could not create seed user.");
                }
            }

            if (!_context.Meters.Any())
            {
                AddMeter($"DAE AS320U-150P Water Meter with Pulse Output", "Rua das Flores", user);
                await _context.SaveChangesAsync();
            }
        }

        private void AddMeter(string name, string address, User user)
        {
            _context.Meters.Add(new Meter
            {
                Name = name,
                Address = address,
                User = user,
            });
        }
    }
}
