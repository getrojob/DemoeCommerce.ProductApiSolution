using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entity;

namespace ProductApi.Infraestructure.Data
{
    public class ProductDbContext : DbContext
    {
        // Explicitly pass options to the base constructor
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
