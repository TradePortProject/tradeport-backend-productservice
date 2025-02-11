using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Data;
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

        public async Task<Product> GetProductByIdAsync(Guid Id)
        {
            return await FindByCondition(product => product.IsActive).OrderBy(x => x.ProductCode).FirstOrDefaultAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
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
            var result = await dbContext.Database.ExecuteSqlRawAsync("SELECT NEXT VALUE FOR ProductCodeSequence");
            return $"P{result:D5}";
        }
    }
}
