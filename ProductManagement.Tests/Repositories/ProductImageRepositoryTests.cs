using Xunit;
using ProductManagement.Repositories;
using ProductManagement.Tests.Helpers;
using ProductManagement.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace ProductManagement.Tests.Repositories
{
    public class ProductImageRepositoryTests
    {
        [Fact]
        public async Task AddProductImageAsync_SavesImageSuccessfully()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var seededProducts = TestDataSeeder.SeedProducts(dbContext);
            var product = seededProducts.First();

            var productImage = new ProductImage
            {
                ImageID = Guid.NewGuid(),
                ProductID = product.ProductID,
                ProductImageURL = "/uploads/test-image.jpg",
                FileName = "test-image.jpg",
                FileExtension = ".jpg"
            };

            var repo = new ProductImageRepository(dbContext);

            // Act
            var result = await repo.AddProductImageAsync(productImage);

            // Assert
            var saved = dbContext.ProductImages.FirstOrDefault(img => img.ImageID == productImage.ImageID);
            Assert.NotNull(saved);
            Assert.Equal("/uploads/test-image.jpg", saved.ProductImageURL);
            Assert.Equal(result.ImageID, saved.ImageID);
        }

        [Fact]
        public async Task GetProductImageByIdAsync_ReturnsImagesForProduct()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var seededProducts = TestDataSeeder.SeedProducts(dbContext);
            var productId = seededProducts.First().ProductID;

            // Seed product image manually
            var productImage = new ProductImage
            {
                ImageID = Guid.NewGuid(),
                ProductID = productId,
                ProductImageURL = "/uploads/product-img.jpg",
                FileName = "product-img.jpg",
                FileExtension = ".jpg"
            };
            dbContext.ProductImages.Add(productImage);
            dbContext.SaveChanges();

            var repo = new ProductImageRepository(dbContext);

            // Act
            var result = await repo.GetProductImageByIdAsync(productId);

            // Assert
            Assert.Single(result);
            Assert.Equal(productId, result[0].ProductID);
            Assert.Equal("product-img.jpg", result[0].FileName);
        }

        [Fact]
        public async Task GetProductImageByIdAsync_ReturnsEmpty_WhenNoImagesFound()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            TestDataSeeder.SeedProducts(dbContext); // No images added
            var repo = new ProductImageRepository(dbContext);
            var unknownProductId = Guid.NewGuid();

            // Act
            var result = await repo.GetProductImageByIdAsync(unknownProductId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddProductImageAsync_ThrowsException_WhenInvalidImage()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var repo = new ProductImageRepository(dbContext);

            var invalidImage = new ProductImage
            {
                ImageID = Guid.NewGuid(),
                ProductID = Guid.NewGuid(), // Valid
                ProductImageURL = null!, // ❌ Required field missing
                FileName = "bad-image.jpg",
                FileExtension = ".jpg"
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await repo.AddProductImageAsync(invalidImage);
            });

            Assert.Contains("Error while saving product image", ex.Message);
        }

    }
}
