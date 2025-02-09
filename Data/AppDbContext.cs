using Microsoft.EntityFrameworkCore;
using TradeportApi.Models;

namespace TradeportApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.WholesalePrice).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.RetailPrice).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ShippingCost).HasColumnType("decimal(10, 2)");
            });
        }
    }
}
