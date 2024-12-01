using System.Linq;
using System.Threading.Tasks;
using UF5423_Aguas.Data.API;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Data
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<Notification> GetNotificationByIdAsync(int id);

        IQueryable<Notification> GetNotifications(string email, string role);

        IQueryable<NotificationDto> ConvertToNotificationDtoAsync(IQueryable<Notification> notifications);
    }
}
