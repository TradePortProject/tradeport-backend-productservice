using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Models;
namespace ProductManagement.Tests.Fixtures
{
    public class ProductTestFixture : IDisposable
    {
        public AppDbContext DbContext { get; private set; }
        public List<Product> Products { get; private set; }
        public ProductTestFixture()
        {
            // Configure the InMemory database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            DbContext = new AppDbContext(options);
            // Initialize test data
            Products = new List<Product>
            {
                new Product
                {
                    ProductID = Guid.NewGuid(),
                    ProductName = "Test Product 1",
                    Description = "Description 1",
                    ProductCode = "P001",
                    RetailCurrency = "USD",
                    WholeSaleCurrency = "USD"
                },
                new Product
                {
                    ProductID = Guid.NewGuid(),
                    ProductName = "Test Product 2",
                    Description = "Description 2",
                    ProductCode = "P002",
                    RetailCurrency = "USD",
                    WholeSaleCurrency = "USD"
                }
            };
            // Seed the database
            SeedDatabase();
        }
        private void SeedDatabase()
        {
            DbContext.Products.AddRange(Products);
            DbContext.SaveChanges();
        }
        public void Dispose()
        {
            // Cleanup the database
            DbContext.Database.EnsureDeleted();
            DbContext.Dispose();
        }
    }
}