using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Data
{
    public class MeterRepository : GenericRepository<Meter>, IMeterRepository
    {
        private readonly DataContext _context;

        public MeterRepository(DataContext context) : base(context)
        {
            _context = context;
        }
    }
}
