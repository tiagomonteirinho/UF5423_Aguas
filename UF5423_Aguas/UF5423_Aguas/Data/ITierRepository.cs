using System.Collections.Generic;
using System.Threading.Tasks;
using UF5423_Aguas.Data.API;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Data
{
    public interface ITierRepository : IGenericRepository<Tier>
    {
        Task<List<Tier>> GetTiersAsync();

        List<TierDto> ConvertToTierDto(IEnumerable<Tier> tiers);
    }
}
