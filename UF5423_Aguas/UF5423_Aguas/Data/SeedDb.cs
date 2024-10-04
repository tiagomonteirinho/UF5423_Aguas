using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
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

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await _userHelper.EnsureCreatedRoleAsync("Admin");
            await _userHelper.EnsureCreatedRoleAsync("Employee");
            await _userHelper.EnsureCreatedRoleAsync("Customer");

            var users = new List<User>();
            var usersToAdd = new List<(string fullName, string email, string role)>
            {
                ("Admin", "admin@mail", "Admin"),
                ("Joaquim", "joaquim@mail", "Employee"),
                ("João", "joao@mail", "Customer"),
                ("Joana", "joana@mail", "Customer"),
                ("Tiago", "tiago@mail", "Customer"),
            };

            foreach (var (fullName, email, role) in usersToAdd)
            {
                var user = await CreateUser(fullName, email, role);
                users.Add(user);
            }

            if (!_context.Meters.Any())
            {
                CreateMeter($"DAE AS320U-150P Water Meter with Pulse Output", "Rua das Flores", users[1]);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<User> CreateUser(string fullName, string email, string role)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FullName = fullName,
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true,
                };

                var result = await _userHelper.RegisterUserAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException($"Could not create seed user.");
                }

                await _userHelper.AddUserToRoleAsync(user, role);
            }

            if (!await _userHelper.IsUserInRoleAsync(user, role))
            {
                await _userHelper.AddUserToRoleAsync(user, role);
            }

            return user;
        }

        private void CreateMeter(string name, string address, User user)
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
