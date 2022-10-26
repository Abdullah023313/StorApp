using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StorApp.Model
{
    public class StorDbContext : IdentityDbContext
    {

        public StorDbContext(DbContextOptions<StorDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Brand> Brands { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(log => Console.WriteLine(log),
                new[] { DbLoggerCategory.Database.Command.Name },
                LogLevel.Information)
                .EnableSensitiveDataLogging();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasQueryFilter(p => !p.IsDeleted);
            base.OnModelCreating(modelBuilder);
        }
    }
}