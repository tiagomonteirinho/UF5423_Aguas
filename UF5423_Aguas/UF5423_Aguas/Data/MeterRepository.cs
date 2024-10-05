using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Helpers;

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

        public async Task<IQueryable<Meter>> GetMetersAsync(string email)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            if (await _userHelper.IsUserInRoleAsync(user, "Employee"))
            {
                return _context.Meters.OrderBy(m => m.User.FullName).ThenByDescending(m => m.Id);
            }

            return _context.Meters.Where(m => m.User == user).OrderByDescending(m => m.Id);
        }
    }
}
