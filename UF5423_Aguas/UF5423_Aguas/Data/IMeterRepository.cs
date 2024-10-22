using System.Linq;
using System.Threading.Tasks;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Models;

namespace UF5423_Aguas.Data
{
    public interface IMeterRepository : IGenericRepository<Meter>
    {
        Task<Meter> GetMeterWithUserByIdAsync(int id);

        Task<IQueryable<Meter>> GetMetersAsync(string email);

        Task<IQueryable<Consumption>> GetConsumptionsAsync(string email);

        Task<Meter> GetMeterWithConsumptionsAsync(int id);

        Task<Consumption> GetConsumptionByIdAsync(int id);

        Task<Consumption> AddConsumptionAsync(ConsumptionViewModel model);

        Task<int> UpdateConsumptionAsync(Consumption consumption);

        Task<int> DeleteConsumptionAsync(Consumption consumption);

        Task<Invoice> ApproveConsumption(Consumption consumption);

        Task<Invoice> GetInvoiceByConsumptionIdAsync(int id);
    }
}
