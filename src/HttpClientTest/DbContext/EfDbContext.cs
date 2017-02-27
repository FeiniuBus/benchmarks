using HttpClientTestCore.Models.Ef;
using Microsoft.EntityFrameworkCore;

namespace HttpClientTestCore.DbContext
{
    public class EfDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public EfDbContext(DbContextOptions<EfDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<EfOpenCity> OpenCities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EfOpenCity>().ToTable("base_opend_city").HasKey(it => it.Adcode);
        }
    }
}