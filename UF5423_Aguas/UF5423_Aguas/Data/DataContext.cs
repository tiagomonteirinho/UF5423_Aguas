using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Meter> Meters { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {

        }
    }
}
