using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterServices_WebApp.Data.Entities;

namespace WaterServices_WebApp.Data
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User> GetByIdAsync(string userId);

        Task DeleteByIdAsync(string userId);
    }
}
