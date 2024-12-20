using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Water_Services.Data.API;
using Water_Services.Data.Entities;

namespace Water_Services.Data
{
    public class TierRepository : GenericRepository<Tier>, ITierRepository
    {
        private readonly DataContext _context;

        public TierRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Tier>> GetTiersAsync()
        {
            return await _context.Tiers.ToListAsync();
        }

        public List<TierDto> ConvertToTierDto(IEnumerable<Tier> tiers)
        {
            return tiers.Select(t => new TierDto
            {
                Id = t.Id,
                VolumeLimit = t.VolumeLimit,
                UnitPrice = t.UnitPrice,
            }).ToList();
        }
    }
}
