using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Meter> Meters { get; set; }

        public DbSet<Consumption> Consumptions { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
    }
}