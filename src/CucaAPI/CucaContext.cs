using CucaAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CucaAPI
{
    public class CucaContext : DbContext
    {
        public DbSet<Cuca> Cuca { get; set; }

        public DbSet<User> User { get; set; }

        public CucaContext(DbContextOptions<CucaContext> opts) : base(opts)
        {
        }
    }
}
