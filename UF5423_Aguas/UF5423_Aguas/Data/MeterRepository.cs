using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Helpers;
using UF5423_Aguas.Models;

namespace UF5423_Aguas.Data
{
    public class MeterRepository : GenericRepository<Meter>, IMeterRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public MeterRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task<IQueryable<Consumption>> GetConsumptionsAsync(string email)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);

            if (await _userHelper.IsUserInRoleAsync(user, "Employee"))
            {
                return _context.Consumptions
                    .Include(c => c.Meter)
                    .ThenInclude(m => m.User)
                    .OrderBy(c => c.Meter.User.FullName)
                    .ThenByDescending(c => c.Meter.Name);
            }

            return _context.Consumptions
                    .Include(c => c.Meter)
                    .ThenInclude(m => m.User)
                    .Where(c => c.Meter.User.Email == email)
                    .OrderBy(c => c.Meter.Name);
        }

        public async Task<Meter> GetMeterWithConsumptionsAsync(int id)
        {
            return await _context.Meters
                .Include(m => m.Consumptions)
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Consumption> GetConsumptionAsync(int id)
        {
            return await _context.Consumptions.FindAsync(id);
        }

        public async Task AddConsumptionAsync(ConsumptionViewModel model)
        {
            var meter = await this.GetMeterWithConsumptionsAsync(model.MeterId);
            if (meter == null)
            {
                return;
            }

            meter.Consumptions.Add(new Consumption
            {
                MeterId = model.MeterId,
                Meter = model.Meter,
                Date = model.Date,
                Volume = model.Volume,
            });

            _context.Meters.Update(meter);
            await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateConsumptionAsync(Consumption consumption)
        {
            var meter = await _context.Meters
                .Where(m => m.Consumptions.Any(c => c.Id == c.Id))
                .FirstOrDefaultAsync();

            _context.Consumptions.Update(consumption);
            await _context.SaveChangesAsync();
            return consumption.Id;
        }

        public async Task<int> DeleteConsumptionAsync(Consumption consumption)
        {
            var meter = await _context.Meters
                .Where(m => m.Consumptions.Any(c => c.Id == c.Id))
                .FirstOrDefaultAsync();

            if (meter == null)
            {
                return 0;
            }

            _context.Consumptions.Remove(consumption);
            await _context.SaveChangesAsync();
            return meter.Id;
        }
    }
}
