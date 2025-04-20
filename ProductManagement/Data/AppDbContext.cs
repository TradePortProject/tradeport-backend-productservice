

using Microsoft.EntityFrameworkCore;
using ProductManagement.Models;  // Import the Product model's namespace

namespace ProductManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        // Define your DbSets here
        public DbSet<Product> Products { get; set; }  // This is the DbSet for the Product entity

        public DbSet<ProductImage> ProductImages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .ToTable("Products");
            modelBuilder.Entity<Product>()
                .Property(b => b.CreatedOn)
                .HasDefaultValueSql("getdate()");
        }
    }
}
