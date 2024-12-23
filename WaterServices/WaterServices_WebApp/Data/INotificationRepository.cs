using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterServices_WebApp.Data.API;
using WaterServices_WebApp.Data.Entities;

namespace WaterServices_WebApp.Data
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<Notification> GetNotificationByIdAsync(int id);

        IQueryable<Notification> GetNotifications(string email, string role);

        List<NotificationDto> ConvertToNotificationDto(IEnumerable<Notification> notifications);
    }
}
