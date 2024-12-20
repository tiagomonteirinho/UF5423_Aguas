using System.Collections.Generic;
using System.Threading.Tasks;
using Water_Services.Data.API;
using Water_Services.Data.Entities;

namespace Water_Services.Data
{
    public interface ITierRepository : IGenericRepository<Tier>
    {
        Task<List<Tier>> GetTiersAsync();

        List<TierDto> ConvertToTierDto(IEnumerable<Tier> tiers);
    }
}
