﻿using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UF5423_Aguas.Data.API;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Data
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

        public IQueryable<NotificationDto> ConvertToNotificationDtoAsync(IQueryable<Notification> notifications)
        {
            var notificationDtos = notifications.Select(m => new NotificationDto
            {
                Id = m.Id,
                Title = m.Title,
                Message = m.Message,
                Action = m.Action,
                Date = m.Date,
                Read = m.Read
            });

            return notificationDtos;
        }
    }
}
