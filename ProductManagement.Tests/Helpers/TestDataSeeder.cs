using System;
using System.Collections.Generic;
using ProductManagement.Models;
using ProductManagement.Data;

namespace ProductManagement.Tests.Helpers
{
    public static class TestDataSeeder
    {
        public static List<Product> SeedProducts(AppDbContext dbContext)
        {
            dbContext.Products.RemoveRange(dbContext.Products);
            dbContext.SaveChanges();

            var products = new List<Product>
            {
                new Product
                {
                    ProductID = Guid.NewGuid(),
                    ProductCode = "P001",
                    ProductName = "Laptop",
                    Description = "Office Laptop",
                    Category = 5,
                    WholesalePrice = 500,
                    RetailPrice = 700,
                    Quantity = 10,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    IsActive = true,
                    RetailCurrency = "USD",
                    WholeSaleCurrency = "USD"
                },
                new Product
                {
                    ProductID = Guid.NewGuid(),
                    ProductCode = "P002",
                    ProductName = "Chair",
                    Description = "Ergonomic chair",
                    Category = 2,
                    WholesalePrice = 50,
                    RetailPrice = 100,
                    Quantity = 50,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    IsActive = true,
                    RetailCurrency = "USD",
                    WholeSaleCurrency = "USD"
                }
            };

            dbContext.Products.AddRange(products);
            dbContext.SaveChanges();

            return products;
        }

        public static List<ProductImage> SeedProductImages(AppDbContext dbContext, Guid productId)
        {
            var images = new List<ProductImage>
            {
                new ProductImage
                {
                    ImageID = Guid.NewGuid(),
                    ProductID = productId,
                    ProductImageURL = "/uploads/images/sample-image.jpg",
                    FileName = "sample-image.jpg",
                    FileExtension = ".jpg"
                }
            };

            dbContext.ProductImages.AddRange(images);
            dbContext.SaveChanges();
            return images;
        }
    }
}