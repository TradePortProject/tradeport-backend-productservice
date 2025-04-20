using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductManagement.Models;
using ProductManagement.Repositories;
using ProductManagement.Tests.Helpers;

namespace ProductManagement.Tests.Repositories
{
    public class RepositoryBaseTests
    {
        [Fact]
        public void Create_AddsEntityToContext()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var repo = new ProductRepository(dbContext);

            var product = new Product
            {
                ProductID = Guid.NewGuid(),
                ProductName = "Created Product",
                ProductCode = "P100",
                Category = 1,
                Description = "New product",
                RetailCurrency = "USD",
                WholeSaleCurrency = "USD",
                WholesalePrice = 100,
                RetailPrice = 150,
                Quantity = 20,
                ShippingCost = 5,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                IsActive = true
            };

            // Act
            repo.Create(product);
            dbContext.SaveChanges();

            // Assert
            var exists = dbContext.Products.Any(p => p.ProductID == product.ProductID);
            Assert.True(exists);
        }

        [Fact]
        public void Update_ChangesEntityInContext()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var products = TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            var product = products.First();
            product.ProductName = "Updated Name";

            // Act
            repo.Update(product);
            dbContext.SaveChanges();

            // Assert
            var updated = dbContext.Products.First(p => p.ProductID == product.ProductID);
            Assert.Equal("Updated Name", updated.ProductName);
        }

        [Fact]
        public void Delete_RemovesEntityFromContext()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var products = TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            var product = products.First();

            // Act
            repo.Delete(product);
            dbContext.SaveChanges();

            // Assert
            var exists = dbContext.Products.Any(p => p.ProductID == product.ProductID);
            Assert.False(exists);
        }
    }
}
