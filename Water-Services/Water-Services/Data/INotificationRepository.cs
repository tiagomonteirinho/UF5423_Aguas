using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Water_Services.Data.API;
using Water_Services.Data.Entities;

namespace Water_Services.Data
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<Notification> GetNotificationByIdAsync(int id);

        IQueryable<Notification> GetNotifications(string email, string role);

        List<NotificationDto> ConvertToNotificationDto(IEnumerable<Notification> notifications);
    }
}
