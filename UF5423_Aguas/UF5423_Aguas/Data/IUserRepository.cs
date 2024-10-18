using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Data
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User> GetByIdAsync(string userId);

        Task DeleteByIdAsync(string userId);

        Task<Notification> GetNotificationByIdAsync(int id);

        IQueryable<Notification> GetNotifications(string email, string role);

        Task UpdateNotificationAsync(Notification notification);
    }
}
