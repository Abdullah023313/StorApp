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
    }
}