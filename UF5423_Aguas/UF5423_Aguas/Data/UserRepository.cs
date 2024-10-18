using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task DeleteByIdAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Notification> GetNotificationByIdAsync(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public IQueryable<Notification> GetNotifications(string email, string role)
        {
            if (email != null)
            {
                return _context.Notifications
                        .Include(n => n.Receiver)
                        .Where(n => n.ReceiverEmail == email)
                        .OrderByDescending(n => n.Date);
            }

            return _context.Notifications
                    .Include(n => n.Receiver)
                    .Where(n => n.ReceiverRole == role)
                    .OrderByDescending(n => n.Date);
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
    }
}
