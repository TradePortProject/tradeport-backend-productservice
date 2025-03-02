using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Models;


namespace ProductManagement.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        private readonly AppDbContext dbContext;
        public ProductRepository(AppDbContext dbContextRepo) : base(dbContextRepo)
        {
            this.dbContext = dbContextRepo;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await FindByCondition(product=> product.IsActive).OrderBy(x => x.CreatedOn).ToListAsync();
        }

        public async Task<List<Product>> GetFilteredProductsAsync(
        string? searchText,
        int? category,
        decimal? minWholesalePrice,
        decimal? maxWholesalePrice,
        decimal? minRetailPrice,
        decimal? maxRetailPrice,
        int? quantity,
        string? sortBy,
        bool? sortDescending,
        int? pageNumber,
        int? pageSize)
        {
            var query = dbContext.Products.Where(p => p.IsActive);

            // Apply filtering
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p =>
                    p.ProductCode.Contains(searchText) ||
                    p.ProductName.Contains(searchText) ||
                    p.Description.Contains(searchText));
            }

            if (category.HasValue)
            {
                query = query.Where(p => p.Category == category.Value);
            }

            if (minWholesalePrice.HasValue)
            {
                query = query.Where(p => p.WholesalePrice >= minWholesalePrice.Value);
            }

            if (maxWholesalePrice.HasValue)
            {
                query = query.Where(p => p.WholesalePrice <= maxWholesalePrice.Value);
            }

            if (minRetailPrice.HasValue)
            {
                query = query.Where(p => p.RetailPrice >= minRetailPrice.Value);
            }

            if (maxRetailPrice.HasValue)
            {
                query = query.Where(p => p.RetailPrice <= maxRetailPrice.Value);
            }

            if (quantity.HasValue)
            {
                query = query.Where(p => p.Quantity >= quantity);
            }

            // Apply sorting
            sortBy = string.IsNullOrWhiteSpace(sortBy) ? "productname" : sortBy.ToLower();
            bool sortDesc = sortDescending ?? false;

            switch (sortBy)
            {
                case "retailprice":
                    query = sortDesc ? query.OrderByDescending(p => p.RetailPrice) : query.OrderBy(p => p.RetailPrice);
                    break;
                case "productname":
                    query = sortDesc ? query.OrderByDescending(p => p.ProductName) : query.OrderBy(p => p.ProductName);
                    break;
                case "category":
                    query = sortDesc ? query.OrderByDescending(p => p.Category) : query.OrderBy(p => p.Category);
                    break;
                default:
                    query = query.OrderBy(p => p.ProductName);
                    break;
            }

            // Apply pagination
            int page = pageNumber ?? 1;
            int size = pageSize ?? 10; //Set defaults pageSize to 10 if not specified.

            query = query.Skip((page - 1) * size).Take(size);

            return await query.ToListAsync();
        }

        public async Task<List<Product>> GetProductByIdAsync(Guid id)
        {
            return await FindByCondition(product => product.ProductID == id && product.IsActive).ToListAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedOn = DateTime.Now;
            product.UpdatedOn = DateTime.Now;
            product.ProductCode = await GetProductCodeAsync();
            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateProductAsync(Guid id, Product product)
        {
            var productObj = await dbContext.Products.FindAsync(id);

            if (productObj == null)
            {
                return null;
            }
            productObj.ManufacturerID = product.ManufacturerID;
            productObj.ProductName = product.ProductName;
            productObj.Description = product.Description;
            productObj.Category = product.Category;
            productObj.WholesalePrice = product.WholesalePrice;
            productObj.RetailPrice = product.RetailPrice;
            productObj.Quantity = product.Quantity;
            productObj.RetailCurrency = product.RetailCurrency;
            productObj.WholeSaleCurrency = product.WholeSaleCurrency;
            productObj.ShippingCost = product.ShippingCost;
            productObj.CreatedOn = product.CreatedOn;
            productObj.UpdatedOn = product.UpdatedOn;
            productObj.IsActive = product.IsActive;

            await dbContext.SaveChangesAsync();
            return productObj;
        }

        public async Task<Product?> UpdateProductQuantityAsync(Guid id, Product updatedProduct)
        {
            var existingProduct = await dbContext.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.Quantity = updatedProduct.Quantity;
            existingProduct.UpdatedOn = DateTime.UtcNow;

            dbContext.Products.Update(existingProduct);
            await dbContext.SaveChangesAsync();

            return existingProduct;
        }


        public async Task<Guid?> DeleteAysnc(Guid Id)
        {
            var productObj = await dbContext.Products.FindAsync(Id);

            if (productObj == null)
            {
                return null;

            }

            dbContext.Products.Remove(productObj);
            await dbContext.SaveChangesAsync();
            return productObj.ProductID;
        }

        public async Task<string> GetProductCodeAsync()
        {
            var lastProductCode = await FindAll().OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.ProductCode).Select(x => x.ProductCode).FirstOrDefaultAsync();
           
            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastProductCode))
            {
                var match = Regex.Match(lastProductCode, @"P(\d+)");
                if (match.Success)
                {
                    nextNumber = int.Parse(match.Groups[1].Value) + 1;
                }

            }
            return $"P{nextNumber:D3}";
        }

        public async Task InsertProductImageAsync(ProductImage productImage)
        {
            dbContext.ProductImages.Add(productImage);
            await dbContext.SaveChangesAsync();
        }
    }
}
