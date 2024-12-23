using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterServices_WebApp.Data.Entities;
using WaterServices_WebApp.Helpers;

namespace WaterServices_WebApp.Data
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

            var users = await _userHelper.GetAllUsersAsync();
            if (users == null || users.Count <= 1) // If only admin or no users exist
            {
                var usersToAdd = new List<(string fullName, string email, string role)>
                {
                    ("Tiago", "admin@mail", "Admin"),
                    ("Joaquim", "employee@mail", "Employee"),
                    ("Joana", "customer@mail", "Customer"),
                    ("Bruno", "customer2@mail", "Customer"),
                };

                foreach (var (fullName, email, role) in usersToAdd)
                {
                    var user = await CreateUser(fullName, email, role);
                    users.Add(user);
                }

                await _context.SaveChangesAsync();
            }

            var meters = _context.Meters.ToList();
            if (meters == null || !meters.Any())
            {
                var metersToAdd = new List<(int serialNumber, string address, User user)>
                {
                    (3427654, "Rua das Flores", users.FirstOrDefault(u => u.Email == "customer@mail")),
                    (1974534, "Rua das Cores", users.FirstOrDefault(u => u.Email == "customer2@mail")),
                };

                foreach (var (serialNumber, address, user) in metersToAdd)
                {
                    var meter = CreateMeter(serialNumber, address, user);
                    meters.Add(meter);
                }

                await _context.SaveChangesAsync();
            }

            if (!_context.Consumptions.Any())
            {
                CreateConsumption(20, "Awaiting approval", meters.FirstOrDefault(m => m.UserEmail == "customer@mail"));
                CreateConsumption(9, "Awaiting approval", meters.FirstOrDefault(m => m.UserEmail == "customer2@mail"));
                await _context.SaveChangesAsync();
            }

            if (!_context.Tiers.Any())
            {
                await CreateTier(0.30M, 5);
                await CreateTier(0.80M, 10);
                await CreateTier(1.20M, 5);
                await CreateTier(1.60M, 9999999);
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
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmAccountAsync(user, token);
            }

            if (!await _userHelper.IsUserInRoleAsync(user, role))
            {
                await _userHelper.AddUserToRoleAsync(user, role);
            }

            return user;
        }

        private Meter CreateMeter(int serialNumber, string address, User user)
        {
            var random = new Random();
            var meter = new Meter()
            {
                SerialNumber = serialNumber,
                Address = address,
                User = user,
                UserEmail = user.Email,
            };

            _context.Meters.Add(meter);
            user.Meters.Add(meter);
            return meter;
        }

        private void CreateConsumption(int volume, string status, Meter meter)
        {
            var consumption = new Consumption()
            {
                Volume = volume,
                Status = status,
                Meter = meter,
                MeterId = meter.Id,
            };

            _context.Consumptions.Add(consumption);
            meter.Consumptions.Add(consumption);
        }

        private async Task CreateTier(decimal unitPrice, int volumeLimit)
        {
            var tier = new Tier()
            {
                UnitPrice = unitPrice,
                VolumeLimit = volumeLimit,
            };

            _context.Tiers.Add(tier);
            await _context.SaveChangesAsync();
        }
    }
}
