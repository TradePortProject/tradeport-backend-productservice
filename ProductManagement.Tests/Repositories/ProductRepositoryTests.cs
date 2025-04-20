using Xunit;
using ProductManagement.Repositories;
using ProductManagement.Tests.Helpers;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Models;

namespace ProductManagement.Tests.Repositories
{
    public class ProductRepositoryTests
    {
        [Fact]
        public async Task GetAllProductsAsync_ReturnsActiveProducts()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var repo = new ProductRepository(dbContext);

            // Act
            var result = await repo.GetAllProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, p => Assert.True(p.IsActive));
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnsCorrectProduct()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var seededProducts = TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            // Act
            var result = await repo.GetProductByIdAsync(seededProducts[0].ProductID);

            // Assert
            Assert.Single(result);
            Assert.Equal(seededProducts[0].ProductID, result[0].ProductID);
        }

        [Fact]
        public async Task GetFilteredProductsAsync_ReturnsAll_WhenNoFilters()
        {
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var products = TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            var result = await repo.GetFilteredProductsAsync(null, null, null, null, null, null, null, false, 1, 10);

            Assert.Equal(products.Count, result.Count);
        }

        [Fact]
        public async Task GetFilteredProductsAsync_ReturnsBySearchText()
        {
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var products = TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            var result = await repo.GetFilteredProductsAsync("Laptop", null, null, null, null, null, null, false, 1, 10);

            Assert.Single(result);
            Assert.Equal("Laptop", result[0].ProductName);
        }

        [Fact]
        public async Task GetFilteredProductsAsync_FiltersByCategory()
        {
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var products = TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            var result = await repo.GetFilteredProductsAsync(null, 2, null, null, null, null, null, false, 1, 10);

            Assert.Single(result);
            Assert.Equal(2, result[0].Category);
        }

        [Fact]
        public async Task GetFilteredProductsAsync_FiltersByWholesalePrice()
        {
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var products = TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            var result = await repo.GetFilteredProductsAsync(null, null, 100, 600, null, null, null, false, 1, 10);

            Assert.Single(result);
            Assert.True(result[0].WholesalePrice >= 100 && result[0].WholesalePrice <= 600);
        }

        [Fact]
        public async Task GetFilteredProductsAsync_FiltersByQuantity()
        {
            var dbContext = DbContextHelper.GetInMemoryDbContext();

            // Ensure clean state
            dbContext.Products.RemoveRange(dbContext.Products);
            await dbContext.SaveChangesAsync();

            var products = TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            var result = await repo.GetFilteredProductsAsync(
                null, null, null, null, null, null, 30, false, 1, 10);

            Assert.Single(result);
            Assert.True(result[0].Quantity >= 30);
        }


        [Fact]
        public async Task GetFilteredProductsAsync_SortsByProductNameAscending()
        {
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            var result = await repo.GetFilteredProductsAsync(null, null, null, null, null, null, null, false, 1, 10);

            Assert.Equal("Chair", result.First().ProductName);
        }

        [Fact]
        public async Task GetFilteredProductsAsync_SortsByProductNameDescending()
        {
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            var result = await repo.GetFilteredProductsAsync(null, null, null, null, null, null, null, true, 1, 10);

            Assert.Equal("Laptop", result.First().ProductName);
        }

        [Fact]
        public async Task GetFilteredProductsAsync_RespectsPagination()
        {
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            var result = await repo.GetFilteredProductsAsync(null, null, null, null, null, null, null, false, 2, 1);

            Assert.Single(result);
            Assert.Equal("Laptop", result[0].ProductName);
        }

        [Fact]
        public async Task GetFilteredProductsAsync_FiltersByMinRetailPrice()
        {
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            // Set a min price that excludes one product
            var result = await repo.GetFilteredProductsAsync(
                null, null, null, null, 600, null, null, false, 1, 10);

            Assert.Single(result);
            Assert.True(result[0].RetailPrice >= 600);
        }

        [Fact]
        public async Task GetFilteredProductsAsync_FiltersByMaxRetailPrice()
        {
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            // Set a max price that excludes one product
            var result = await repo.GetFilteredProductsAsync(
                null, null, null, null, null, 150, null, false, 1, 10);

            Assert.Single(result);
            Assert.True(result[0].RetailPrice <= 150);
        }

        [Fact]
        public async Task GetFilteredProductsAsync_FiltersByRetailPriceRange()
        {
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            var result = await repo.GetFilteredProductsAsync(
                null, null, null, null, 100, 800, null, false, 1, 10);

            Assert.Equal(2, result.Count); // All products fall in this range
            Assert.All(result, p => Assert.InRange(p.RetailPrice ?? 0, 100, 800));
        }

        [Fact]
        public async Task CreateProductAsync_SavesProductAndSetsFields()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var repo = new ProductRepository(dbContext);

            var newProduct = new Product
            {
                ProductName = "New Product",
                Description = "Test Description",
                Category = 1,
                WholesalePrice = 100,
                RetailPrice = 150,
                Quantity = 10,
                RetailCurrency = "USD",
                WholeSaleCurrency = "USD",
                IsActive = true
            };

            // Act
            var result = await repo.CreateProductAsync(newProduct);

            // Assert
            var saved = dbContext.Products.FirstOrDefault(p => p.ProductID == result.ProductID);
            Assert.NotNull(saved);

            // ProductCode is generated
            Assert.False(string.IsNullOrWhiteSpace(result.ProductCode));
            Assert.StartsWith("P", result.ProductCode);

            // Timestamps are set
            Assert.True((DateTime.Now - result.CreatedOn).TotalSeconds < 10);
            Assert.True((DateTime.Now - result.UpdatedOn).TotalSeconds < 10);

            // Data matches input
            Assert.Equal("New Product", result.ProductName);
            Assert.Equal(100, result.WholesalePrice);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdatesProductFieldsSuccessfully()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var seededProducts = TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            var original = seededProducts[0];
            var updatedProduct = new Product
            {
                ManufacturerID = Guid.NewGuid(),
                ProductName = "Updated Name",
                Description = "Updated Desc",
                Category = 3,
                WholesalePrice = 222,
                RetailPrice = 333,
                Quantity = 5,
                RetailCurrency = "EUR",
                WholeSaleCurrency = "EUR",
                ShippingCost = 10.5m,
                CreatedOn = original.CreatedOn, // keep original
                UpdatedOn = DateTime.UtcNow,
                IsActive = false
            };

            // Act
            var result = await repo.UpdateProductAsync(original.ProductID, updatedProduct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.ProductName);
            Assert.Equal(333, result.RetailPrice);
            Assert.Equal("EUR", result.RetailCurrency);
            Assert.Equal(10.5m, result.ShippingCost);
            Assert.False(result.IsActive);
        }

        [Fact]
        public async Task UpdateProductAsync_ReturnsNull_WhenProductNotFound()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            var nonExistentId = Guid.NewGuid();
            var dummyProduct = new Product
            {
                ProductName = "Doesn't Matter",
                UpdatedOn = DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            };

            // Act
            var result = await repo.UpdateProductAsync(nonExistentId, dummyProduct);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateProductQuantityAsync_UpdatesQuantitySuccessfully()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var products = TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            var original = products[0];
            var updated = new Product
            {
                Quantity = 99,
                UpdatedOn = DateTime.UtcNow
            };

            // Act
            var result = await repo.UpdateProductQuantityAsync(original.ProductID, updated);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(99, result.Quantity);
            Assert.True((DateTime.UtcNow - result.UpdatedOn).TotalSeconds < 10);
        }

        [Fact]
        public async Task UpdateProductQuantityAsync_ReturnsNull_WhenProductNotFound()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);

            var nonExistingId = Guid.NewGuid();
            var updated = new Product { Quantity = 42 };

            // Act
            var result = await repo.UpdateProductQuantityAsync(nonExistingId, updated);

            // Assert
            Assert.Null(result);
        }

        private static ProductRepository SetupWithSeededData()
        {
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            TestDataSeeder.SeedProducts(dbContext);
            return new ProductRepository(dbContext);
        }

        [Fact]
        public async Task GetTotalProductCountAsync_ReturnsAll_WhenNoFilters()
        {
            var repo = SetupWithSeededData();

            var result = await repo.GetTotalProductCountAsync(null, null, null, null, null, null, null);

            Assert.Equal(2, result); // 2 from TestDataSeeder
        }

        [Fact]
        public async Task GetTotalProductCountAsync_FiltersBySearchText()
        {
            var repo = SetupWithSeededData();

            var result = await repo.GetTotalProductCountAsync("Laptop", null, null, null, null, null, null);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetTotalProductCountAsync_FiltersByCategory()
        {
            var repo = SetupWithSeededData();

            var result = await repo.GetTotalProductCountAsync(null, 5, null, null, null, null, null); // Laptop category

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetTotalProductCountAsync_FiltersByMinWholesalePrice()
        {
            var repo = SetupWithSeededData();

            var result = await repo.GetTotalProductCountAsync(null, null, 200, null, null, null, null);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetTotalProductCountAsync_FiltersByMaxWholesalePrice()
        {
            var repo = SetupWithSeededData();

            var result = await repo.GetTotalProductCountAsync(null, null, null, 100, null, null, null);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetTotalProductCountAsync_FiltersByMinRetailPrice()
        {
            var repo = SetupWithSeededData();

            var result = await repo.GetTotalProductCountAsync(null, null, null, null, 500, null, null);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetTotalProductCountAsync_FiltersByMaxRetailPrice()
        {
            var repo = SetupWithSeededData();

            var result = await repo.GetTotalProductCountAsync(null, null, null, null, null, 150, null);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetTotalProductCountAsync_FiltersByQuantity()
        {
            var repo = SetupWithSeededData();

            var result = await repo.GetTotalProductCountAsync(null, null, null, null, null, null, 30);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetTotalProductCountAsync_CombinesAllFilters()
        {
            var repo = SetupWithSeededData();

            var result = await repo.GetTotalProductCountAsync(
                "Laptop", 5, 100, 600, 500, 800, 5);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task DeleteAsync_RemovesProductAndReturnsId()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var seeded = TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);
            var productToDelete = seeded[0];

            // Act
            var result = await repo.DeleteAysnc(productToDelete.ProductID);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productToDelete.ProductID, result);
            Assert.False(dbContext.Products.Any(p => p.ProductID == productToDelete.ProductID));
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNull_WhenProductNotFound()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            TestDataSeeder.SeedProducts(dbContext);
            var repo = new ProductRepository(dbContext);
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await repo.DeleteAysnc(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductCodeAsync_ReturnsP001_WhenNoProductsExist()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext(); // empty db
            var repo = new ProductRepository(dbContext);

            // Act
            var code = await repo.GetProductCodeAsync();

            // Assert
            Assert.Equal("P001", code);
        }

        [Fact]
        public async Task GetProductCodeAsync_ReturnsNextSequentialCode()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            TestDataSeeder.SeedProducts(dbContext); // seeds P001 and P002
            var repo = new ProductRepository(dbContext);

            // Act
            var code = await repo.GetProductCodeAsync();

            // Assert
            Assert.Equal("P003", code);
        }

        [Fact]
        public async Task InsertProductImageAsync_SavesImageSuccessfully()
        {
            // Arrange
            var dbContext = DbContextHelper.GetInMemoryDbContext();
            var seededProducts = TestDataSeeder.SeedProducts(dbContext);
            var productId = seededProducts[0].ProductID;

            var image = new ProductImage
            {
                ImageID = Guid.NewGuid(),
                ProductID = productId,
                ProductImageURL = "/uploads/test-image.jpg",
                FileName = "test-image.jpg",
                FileExtension = ".jpg"
            };

            var repo = new ProductRepository(dbContext);

            // Act
            await repo.InsertProductImageAsync(image);

            // Assert
            var savedImage = dbContext.ProductImages.FirstOrDefault(img => img.ImageID == image.ImageID);
            Assert.NotNull(savedImage);
            Assert.Equal(productId, savedImage.ProductID);
            Assert.Equal(".jpg", savedImage.FileExtension);
        }


    }
}

