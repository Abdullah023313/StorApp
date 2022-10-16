using Microsoft.EntityFrameworkCore;

namespace StorApp.Model
{
    public class StorDbContext : DbContext
    {

        public StorDbContext(DbContextOptions<StorDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Brand> Brands { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(log => Console.WriteLine(log),
                new[] {DbLoggerCategory.Database.Command.Name},
                LogLevel.Information);
        }
    }
}