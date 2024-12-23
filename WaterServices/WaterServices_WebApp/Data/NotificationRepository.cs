using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterServices_WebApp.Data.API;
using WaterServices_WebApp.Data.Entities;

namespace WaterServices_WebApp.Data
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly DataContext _context;

        public NotificationRepository(DataContext context) : base(context)
        {
            _context = context;
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

        public List<NotificationDto> ConvertToNotificationDto(IEnumerable<Notification> notifications)
        {
            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Date = n.Date,
                Read = n.Read
            }).ToList();
        }
    }
}
