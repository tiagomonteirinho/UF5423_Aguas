using System.Linq;
using System.Threading.Tasks;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Data
{
    public interface IMeterRepository : IGenericRepository<Meter>
    {
        Task<IQueryable<Meter>> GetMetersAsync(string email);
    }
}
