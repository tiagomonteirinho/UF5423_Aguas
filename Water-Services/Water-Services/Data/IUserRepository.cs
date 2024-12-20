using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Water_Services.Data.Entities;

namespace Water_Services.Data
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();

        Task<User> GetByIdAsync(string userId);

        Task DeleteByIdAsync(string userId);
    }
}
