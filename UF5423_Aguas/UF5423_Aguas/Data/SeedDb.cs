﻿using Microsoft.AspNetCore.Identity;
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
            //_random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            var user = await _userHelper.GetUserByEmailAsync("admin@mail.com");
            if (user == null)
            {
                user = new User
                {
                    FullName = "Tiago Monteirinho",
                    Email = "admin@mail.com",
                    UserName = "admin@mail.com",
                };

                var result = await _userHelper.AddUserAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    //throw new InvalidOperationException($"Could not create seed user account. \n Errors: {result.Errors.ToString()}");
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Could not create seed user account. Errors: {errors}");
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