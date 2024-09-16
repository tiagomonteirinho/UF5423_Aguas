using System;
using System.Linq;
using System.Threading.Tasks;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private Random _random;

        public SeedDb(DataContext context)
        {
            _context = context;
            //_random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            if (!_context.Meters.Any())
            {
                AddMeter($"DAE AS320U-150P Water Meter with Pulse Output", "Rua das Flores");
                await _context.SaveChangesAsync();
            }
        }

        private void AddMeter(string name, string address)
        {
            _context.Meters.Add(new Meter
            {
                Name = name,
                Address = address
            });
        }
    }
}
