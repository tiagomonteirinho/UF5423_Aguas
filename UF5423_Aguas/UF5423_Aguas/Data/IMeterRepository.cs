using System.Linq;
using System.Threading.Tasks;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Models;

namespace UF5423_Aguas.Data
{
    public interface IMeterRepository : IGenericRepository<Meter>
    {
        Task<IQueryable<Consumption>> GetConsumptionsAsync(string email);

        Task<Meter> GetMeterWithConsumptionsAsync(int id);

        Task<Consumption> GetConsumptionAsync(int id);

        Task AddConsumptionAsync(ConsumptionViewModel model);

        Task<int> UpdateConsumptionAsync(Consumption consumption);

        Task<int> DeleteConsumptionAsync(Consumption consumption);
    }
}
