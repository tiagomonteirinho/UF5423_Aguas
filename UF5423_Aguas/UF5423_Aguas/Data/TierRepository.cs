using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Data
{
    public class TierRepository : GenericRepository<Tier>, ITierRepository
    {
        private readonly DataContext _context;

        public TierRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
