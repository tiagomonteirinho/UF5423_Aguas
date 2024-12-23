using System.Collections.Generic;
using System.Threading.Tasks;
using WaterServices_WebApp.Data.API;
using WaterServices_WebApp.Data.Entities;

namespace WaterServices_WebApp.Data
{
    public interface ITierRepository : IGenericRepository<Tier>
    {
        Task<List<Tier>> GetTiersAsync();

        List<TierDto> ConvertToTierDto(IEnumerable<Tier> tiers);
    }
}
