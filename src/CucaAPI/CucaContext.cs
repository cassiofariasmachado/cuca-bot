using CucaAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CucaAPI
{
    public class CucaContext : DbContext
    {
        public DbSet<Cuca> Cucas { get; set; }

        public DbSet<User> Users { get; set; }

        public CucaContext(DbContextOptions<CucaContext> opts) : base(opts)
        {
        }
    }
}
