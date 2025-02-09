

using Microsoft.EntityFrameworkCore;
using ProductManagement.Models;  // Import the Product model's namespace

namespace ProductManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        // Define your DbSets here
        public DbSet<Product> Products { get; set; }  // This is the DbSet for the Product entity
    }
}
